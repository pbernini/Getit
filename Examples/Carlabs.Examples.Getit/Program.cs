using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL.Common.Response;
using Newtonsoft.Json.Linq;
using ConsoleDump;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
#pragma warning disable IDE1006
namespace Carlabs.Getit.Examples
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Dealer
    {
        public int id { get; set; }
        public int subId { get; set; }
        public string name { get; set; }
        public string make { get; set; }
        public int makeId { get; set; }
        public string dealershipHours { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string county { get; set; }
        public string phone { get; set; }
        public string website { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string internetManager { get; set; }
        public string contactEmail { get; set; }
        public string dteUpdated { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public string _debug { get; set; }
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class NearestDealer
    {
        public Dealer Dealer;
        public double distance;
    }
#pragma warning restore IDE1006 // Naming Styles

    public class TestDealers
    {
        public IList<Dealer> Dealers { get; set; }
    }

    /// <summary>
    ///  Sample program shows some use patterns. NOTE if the data doesn't
    ///  exist for the queries you will get exceptions. This is set up
    ///  for Test data.
    /// </summary>
    // ReSharper disable once ArrangeTypeModifiers
    // ReSharper disable once ClassNeverInstantiated.Global
    class Program
    {
        // need language version 7.1+ to do async on main
        // ReSharper disable once UnusedParameter.Local
        private static async Task Main(string[] args)
        {
            // Arrange
            // NOTE : THIS TEST WILL FAIL WITHOUT A VALID WORKING GQL ENDPOINT WITH UPDATE QUERY
            Getit getit = new Getit();
            Config config = new Config();

            // Set a URL to your graphQL endpoint
            config.SetUrl("https://clapper.kia-dev.car-labs.com/graphql");

            string raw = @"{Make(name: ""Kia"") {id name }}";
            IQuery jQuery = getit.Query();
            jQuery.Raw(raw);

            // now call the get with config, and the query. Query can be decouple from config!

            JObject jOb = await getit.Get<JObject>(jQuery, config);
            Console.WriteLine(jOb);

            IQuery subSelect = getit.Query();

            // set up a couple of enums for testing

            EnumHelper gqlEnumEnabled = new EnumHelper().Enum("ENABLED");
            EnumHelper gqlEnumDisabled = new EnumHelper("DISABLED");
            gqlEnumDisabled.Enum("SOMETHING_ENUM");

            // simple sub selection list

            List<object> subSelList = new List<object>(new object[] {"subName", "subMake", "subModel"});

            // Where (params) part with enums

            Dictionary<string, object> mySubDict = new Dictionary<string, object>
            {
                {"subMake", "aston martin"},
                {"subState", "ca"},
                {"subLimit", 1},
                {"_debug", gqlEnumDisabled},
                {"SuperQuerySpeed", gqlEnumEnabled}
            };

            // create the sub-select part for later use

            subSelect
                .Select(subSelList)
                .Name("subDealer")
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
                {"make", "aston martin"},
                {"state", "ca"},
                {"limit", 2},
                {"trims", trimList},
                {"models", modelList},
                {"price", fromToPrice},
                {"_debug", gqlEnumEnabled},
            };

            // finally build some big query with all that stuff

            IQuery query = getit.Query();

            query
                .Name("Dealers")
                .Select("somemore", "things", "inaselect")
                .Select(selList)
                .Alias("myDealerAlias")
                .Where(myDict)
                .Where("id_int", 1)
                .Where("id_double", 3.25)
                .Where("id_string", "some_sting_id")
                .Comment("My First GQL Query with getit\na second line of comments\nand yet another line of comments");

            Console.WriteLine($"{query}");

            // just dump the sub-select alone adding an alias

            subSelect.Alias("myDealerSubSelect");
            Console.WriteLine($"{subSelect}");

            // clear it all so we can start over with a REAL example

            query.Clear();
            subSelect.Clear();

            subSelect
                .Name("Dealer")
                .Select("id", "subId", "name", "make", "makeId", "dealershipHours", "address")
                .Select("city", "state", "zip", "county", "phone", "website", "latitude", "longitude")
                .Select("internetManager", "contactEmail", "dteUpdated", "type", "status");

            IQuery nearestDealerQuery = getit.Query();
            IQuery batchQuery = getit.Query();

            nearestDealerQuery
                .Name("NearestDealer")
                .Alias("TheNearest")
                .Select("distance")
                .Select(subSelect)
                .Where("zip", "91302")
                .Where("makeId", 16);

            Console.WriteLine("Nearest Dealer Query");
            Console.WriteLine(nearestDealerQuery);
            Console.WriteLine("Testing String Get");
            Console.WriteLine(await getit.Get<string>(nearestDealerQuery, config));
            Console.WriteLine("Done with String Get");

            Console.WriteLine("Testing NearestDealer Get");
            List<NearestDealer> objResults = await getit.Get<List<NearestDealer>>(nearestDealerQuery, config, "TheNearest");
            objResults.Dump();
            Console.WriteLine("Done with NearestDealer Get");

            string rawQuery = nearestDealerQuery.ToString();

            batchQuery.Raw(rawQuery);
            nearestDealerQuery.Alias("batchedNearest");
            batchQuery.Batch(nearestDealerQuery);

            Console.WriteLine("Testing Batch Raw Query Get");
            Console.WriteLine(await getit.Get<string>(batchQuery, config));
            Console.WriteLine("Done with Batch Raw Query Get");
            Console.WriteLine("Batched Query String -");
            Console.WriteLine(batchQuery.ToString());

            nearestDealerQuery.Clear();
            nearestDealerQuery
                .Name("NearestDealer")
                .Alias("TheNearest")
                .Select("distance")
                .Select(subSelect)
                .Where("zip", "91302")
                .Where("makeId", 16);

            Console.WriteLine("Testing NearestDealer Query Get with Error Check");
            objResults = await getit.Get<List<NearestDealer>>(nearestDealerQuery, config, "TheNearest");

            if(objResults == null)
                Console.WriteLine("No Data in Results");
            else
                objResults.Dump();

            // check the query to see if it captured any errors

            if (nearestDealerQuery.HasErrors())
            {
                foreach (GraphQLError gqlErr in nearestDealerQuery.GqlErrors)
                {
                    Console.WriteLine("Error : " + gqlErr.Message);
                    foreach (GraphQLLocation loc in gqlErr.Locations)
                    {
                        Console.WriteLine("  -->Location Line : " + loc.Line + ", Column : " + loc.Column);
                    }
                }
            }
            else
            {
                Console.WriteLine("No Errors Found");
            }

            Console.WriteLine("Done with NearestDealer Query Get with Error Check");

            Console.WriteLine("Begin Testing with JObject");
            //QueryStringBuilder jsonQueryString = new QueryStringBuilder();
            //Query jsonQuery = new Query(jsonQueryString, config);

            // NOTE : THIS TEST WILL FAIL WITHOUT A VALID WORKING GQL ENDPOINT WITH UPDATE QUERY
            config.SetUrl("https://clapper.honda-dev.car-labs.com/graphql");
            IQuery jsonQuery = getit.Query();

            jsonQuery.Raw(rawQuery);
            JObject jO = await getit.Get<JObject>(jsonQuery, config);
            Console.WriteLine(jO);
            Console.WriteLine(jO.Value<JArray>("TheNearest")[0].Value<double>("distance"));

            Console.WriteLine("End Testing with JObject");
        }
    }
}
