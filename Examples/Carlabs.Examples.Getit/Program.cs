using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using GraphQL.Client;
using GraphQL.Common.Request;
using GraphQL.Common.Response;

using Carlabs.Getit;

namespace Carlabs.Examples.Getit
{
    public class GqlTester
    {
        public GraphQLResponse GqlResp { get; set; }
        public string Url { get; set; }

        public async Task Test(string gqlQuery)
        {
            GraphQLRequest aQuery = new GraphQLRequest
            {
                Query = gqlQuery
            };

            GraphQLClient graphQlClient = new GraphQLClient(Url);

            try
            {
                GqlResp = await graphQlClient.PostAsync(aQuery);
            }
            catch (Exception)
            {
                GqlResp = null;
            }
        }
    }

    class Program
    {
        // need language version 7.1+ to do async on main 
        // ReSharper disable once UnusedParameter.Local
        static async Task Main(string[] args)
        {
            QueryStringBuilder subSelectString = new QueryStringBuilder();
            Query subSelect = new Query(subSelectString);

            // set up a couple of enums for testing

            EnumHelper gqlEnumEnabled = new EnumHelper().Enum("ENABLED");
            EnumHelper gqlEnumDisabled = new EnumHelper("DISABLED");
            gqlEnumDisabled.Enum("SOMETHNG_ENUM");

            // simple sub selection list

            List<object> subSelList = new List<object>(new object[] {"subName", "subMake", "subModel"});

            // Where (params) part with enums

            Dictionary<string, object> mySubDict = new Dictionary<string, object>
            {
                {"subMake", "honda"},
                {"subState", "ca"},
                {"subLimit", 1},
                {"__debug", gqlEnumDisabled},
                {"SuperQuerySpeed", gqlEnumEnabled}
            };

            // create the sub-select part for later use

            subSelect
                .Select(subSelList)
                .From("subDealer")
                .Where(mySubDict)
                .Comment("SubSelect Below!");

            // make a list with a sub-list

            List<object> selList = new List<object>(new object[] {"id", subSelect, "name", "make", "model"});

            // try list of ints and list of strings

            List<int> trimList = new List<int>(new[] {43783, 43784, 43145});
            List<string> modelList = new List<string>(new[] {"DB7", "DB9", "Vantage"});
            List<object> recList = new List<object>(new object[] {"aa", "bb", "cc"});

            // try a dict for the typical from to

            Dictionary<string, object> recMap = new Dictionary<string, object>
            {
                {"from", 444.45},
                {"to", 555.45},
            };

            // try a more complicate dict with sub structs, list and map

            Dictionary<string, object> fromToPrice = new Dictionary<string, object>
            {
                {"from", 123},
                {"to", 454},
                {"recurse", recList},
                {"map", recMap}
            };

            Dictionary<string, object> myDict = new Dictionary<string, object>
            {
                {"make", "honda"},
                {"state", "ca"},
                {"limit", 2},
                {"trims", trimList},
                {"models", modelList},
                {"price", fromToPrice},
                {"__debug", gqlEnumEnabled},
            };

            // finally build some big query with all that stuff

            QueryStringBuilder queryString = new QueryStringBuilder();
            Query query = new Query(queryString);

            query
                .From("Dealers")
                .Select("somemore", "things", "inaselect")
                .Select(selList)
                .Alias("myDealerAlias")
                .Where(myDict)
                .Where("id_int", 1)
                .Where("id_double", 3.25)
                .Where("id_string", "some_sting_id")
                .Comment("My First F'n GQL Query with geTit\na second line of comments\nand yet another line of comments");

            Console.WriteLine($"{query}");

            // just dump the sub-select alone adding an alias

            subSelect.Alias("myDealerSubSelect");
            Console.WriteLine($"{subSelect}");

            // clear it all so we can start over with a REAL example

            query.Clear();

            query
                .From("Dealer")
                .Alias("TestDealers")
                .Select("id", "subId", "name", "make", "makeId", "dealershipHours", "address")
                .Select("city", "state", "zip", "county", "phone", "website", "latitude", "longitude")
                .Select("internetManager", "contactEmail", "dteUpdated", "type", "status", "__debug")
                .Where("limit", 3)
                .Where("__debug", gqlEnumEnabled);

            // see the shipped query

            Console.WriteLine(query);

            GqlTester testGql = new GqlTester
            {
                // set the URL of a clapi gql enabled server here

                Url = "http://192.168.1.75/clapper/web/graphql"
            };

            try
            {
                await testGql.Test("{" + query + "}");
            }
            catch (Exception e)
            {
                Console.WriteLine("In Main");
                throw;
            }

            // dump some data if any

            if (testGql.GqlResp != null)
            {
                if (testGql.GqlResp.Data != null)
                {
                    Console.WriteLine(testGql.GqlResp.Data.ToString());
                }
                else
                {
                    Console.WriteLine("Null Returned for data, Query or Server Issue");
                }

                // peel off some errors if any

                if (testGql.GqlResp.Errors == null)
                {
                    Console.WriteLine("No GrapQL Errors");
                }
                else
                {
                    Console.WriteLine("Completed with Errors - ");
                    foreach (var err in testGql.GqlResp.Errors)
                    {
                        Console.WriteLine("  " + err.Message);
                        foreach (var location in err.Locations)
                        {
                            Console.WriteLine("  Line   --> " + location.Line);
                            Console.WriteLine("  Column --> " + location.Column);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid Respone to Query, Possible Sever Error");
            }
        }
    }
}
