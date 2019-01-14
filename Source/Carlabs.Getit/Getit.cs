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
        /// <exception cref="ArgumentNullException">Invalid Configuration</exception>
        public async Task<T> Get<T>(IQuery query, IConfig config, string resultName = null)
        {
            // better have a solid config or expect something bad

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            // Set up the needed client stuff

            GraphQLClient gqlClient = new GraphQLClient(config.Url);

            // Generate the query can throw here

            GraphQLRequest gqlQuery = new GraphQLRequest {Query = "{" + query.ToString() + "}"};

            // make the call to the server, this will toss on any non 200 response

            GraphQLResponse gqlResp = await gqlClient.PostAsync(gqlQuery);

            // check for no results, this is an odd case but should be caught

            // Any mising/empty data or response errors (GQL) will cause an exception!
            //
            // NOTE: GQL can return VALID data for a partial set of queries and errors
            // for others all in the same response set. Our case here is that ANY errors cause
            // a report of failure.

            if (gqlResp?.Data == null || gqlResp.Errors != null)
            {
                ArgumentException ex = new ArgumentException("GQLResponse Data");
                ex.Data.Add("request", gqlQuery.ToString());

                // If we have any Errors reports, we are tossing so pass them back too

                if (gqlResp?.Errors != null && gqlResp.Errors.Length > 0)
                {
                    ex.Data.Add("gqlErrors", gqlResp.Errors);
                }

                throw ex;
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
