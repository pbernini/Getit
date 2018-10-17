using System;
using System.Threading.Tasks;
using GraphQL.Client;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Carlabs.Getit
{
    public class Getit : IGetit
    {
        private IConfig _config;

        /// <summary>
        /// Config storage for Getit, optionally can be used for
        /// setting config on Get() calls.
        /// </summary>
        public IConfig Config
        {
            get => _config;
            set => _config = value ?? throw new ArgumentNullException(nameof(value));
        }

        public Getit()
        {
            _config = null;
        }

        /// <summary>
        /// Set up anything needed, in this case we can
        /// optionally passin the config that will be useful
        /// for subsequent Get() calls if not supplied.
        /// </summary>
        /// <param name="config"></param>
        public Getit(IConfig config)
        {
            Config = config;
        }

        /// <inheritdoc />
        /// <summary>
        /// The Query Factory/Dispenser. These are independant
        /// of connection so you can create a bunch and use them
        /// for sub-queries, etc.
        /// </summary>
        /// <returns></returns>
        public IQuery Query()
        {
            return new Query();
        }

        /// <summary>
        /// Given a type return the results of a GraphQL query in it. If
        /// the type is a string then will return the JSON string. The resultName
        /// will be automatically set the Name or Alias name if not specified.
        /// For Raw queries you must set the resultName param OR set the Name() in
        /// the query to match. This handles server connection here!
        /// </summary>
        /// <typeparam name="T">Data Type, typically a list of the record but not always.
        /// </typeparam>
        /// <param name="query"></param>
        /// <param name="resultName">Overide of the Name/Alias of the query</param>
        /// <returns>The type of object stuffed with data from the query</returns>
        /// <exception cref="ArgumentException">Dupe Key, missing parts or empty parts of a query</exception>
        public async Task<T> Get<T>(IQuery query, string resultName = null)
        {
            return await Get<T>(query, Config, resultName);
        }

        /// <summary>
        /// Given a type return the results of a GraphQL query in it. If
        /// the type is a string then will return the JSON string. The resultName
        /// will be automatically set the Name or Alias name if not specified.
        /// For Raw queries you must set the resultName param OR set the Name() in
        /// the query to match. This handles server connection here!
        /// </summary>
        /// <typeparam name="T">Data Type, typically a list of the record but not always.
        /// </typeparam>
        /// <param name="query"></param>
        /// <param name="config"></param>
        /// <param name="resultName">Overide of the Name/Alias of the query</param>
        /// <returns>The type of object stuffed with data from the query</returns>
        /// <exception cref="ArgumentException">Dupe Key, missing parts or empty parts of a query</exception>
        public async Task<T> Get<T>(IQuery query, IConfig config, string resultName = null)
        {
            if (config == null)
            {
                throw new ArgumentNullException("Invalid Config, it's null and should not be. Check Getit config or pass in a valid one");
            }

            // Set up the needed client stuff
            GraphQLClient gqlClient = new GraphQLClient(config.Url);

            // Generate the query can throw here
            GraphQLRequest gqlQuery = new GraphQLRequest {Query = "{" + query.ToString() + "}"};
            GraphQLResponse gqlResp;

            try
            {
                // make the call to the server, bounce on bad errors
                // could bubble up the exception if needed

                gqlResp = await gqlClient.PostAsync(gqlQuery);
            }
            catch (Exception)
            {
                // TODO: This may get changed to a re-throw or not catch it
                gqlResp = null;
            }

            // check for no results, return null in object flavor

            if (gqlResp == null)
            {
                return (T)(object)null;
            }

            // see if any Gql errors, can be 0 to many

            if (gqlResp.Errors != null)
            {
                if (gqlResp.Errors.Length > 0)
                {
                    // lets move these to our instance data

                    query.GqlErrors.AddRange(gqlResp.Errors);
                }
            }

            // Must also check for null on the data portion, but
            // only AFTER the error check so they can be processed

            if (gqlResp.Data == null)
            {
                return (T)(object)null;
            }

            // If the given type was a string, ship the raw JSON string

            if (typeof(T) == typeof(string))
            {
                return gqlResp.Data.ToString();
            }

            // If the smart user passes in a JObject, get it that way instead of fixed T type
            // Your (T) must match the structure of the JSON being returned or expect an exception

            if (typeof(T) == typeof(JObject))
            {
                return JsonConvert.DeserializeObject<JObject>(gqlResp.Data.ToString());
            }

            // Now we need to get the results name. This is EITHER the Name, or the Alias
            // name. If Alias was set then use it. If the user does specify it in
            // the Get call it's an overide. This might be needed with raw query

            if (resultName == null)
            {
                resultName = string.IsNullOrWhiteSpace(query.AliasName) ? query.QueryName : query.AliasName;
            }

            // Let the client do the mapping , all sorts of things can thow at this point!
            // caller should check for exceptions, Generally invalid mapping into the type

            return gqlResp.GetDataFieldAs<T>(resultName);
        }
    }
}
