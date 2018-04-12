using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using GraphQL.Client;
using GraphQL.Common.Request;
using GraphQL.Common.Response;

using Carlabs.Getit;
using ConsoleDump;

namespace Carlabs.Examples.Getit
{
#pragma warning disable IDE1006 // Naming Styles off for GQL structs
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
        public string __debug { get; set; }
    }

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
    
    class Program
    {
        // need language version 7.1+ to do async on main 
        // ReSharper disable once UnusedParameter.Local
        private static async Task Main(string[] args)
        {
            QueryStringBuilder subSelectString = new QueryStringBuilder();
            Query subSelect = new Query(subSelectString);

            // set up a couple of enums for testing

            EnumHelper gqlEnumEnabled = new EnumHelper().Enum("ENABLED");
            EnumHelper gqlEnumDisabled = new EnumHelper("DISABLED");
            gqlEnumDisabled.Enum("SOMETHING_ENUM");

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
                .Name("Dealers")
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
            subSelect.Clear();

            subSelect
                .Name("Dealer")
                .Select("id", "subId", "name", "make", "makeId", "dealershipHours", "address")
                .Select("city", "state", "zip", "county", "phone", "website", "latitude", "longitude")
                .Select("internetManager", "contactEmail", "dteUpdated", "type", "status");

            QueryStringBuilder nearestDealerQueryString = new QueryStringBuilder();
            Query nearestDealerQuery = new Query(nearestDealerQueryString);

            nearestDealerQuery
                .Name("NearestDealer")
                .Alias("XXXXXX")
                .Select("distance")
                .Select(subSelect)
                .Where("zip", "91302")
                .Where("makeId", 16);

            Console.WriteLine(nearestDealerQuery);

            Console.WriteLine("Testing String Get");
            Console.WriteLine(await nearestDealerQuery.Get<string>());
            Console.WriteLine("Done with String Get");

            Console.WriteLine("Testing NearestDealer Get");
            List<NearestDealer> objResults = await nearestDealerQuery.Get<List<NearestDealer>>("XXXXXX");
            objResults.Dump();
            Console.WriteLine("Done with NearestDealer Get");

            nearestDealerQuery.Alias();
            nearestDealerQuery.Select("bogusField");

            string rawQuery = nearestDealerQuery.ToString();

            nearestDealerQuery.Clear();
            nearestDealerQuery.Raw(rawQuery);

            Console.WriteLine("Testing Raw Query Get");
            Console.WriteLine(await nearestDealerQuery.Get<string>());
            Console.WriteLine("Done with Raw Query Get");

            Console.WriteLine("Testing Raw NearestDealer Query Get");
            objResults = await nearestDealerQuery.Get<List<NearestDealer>>("NearestDealer");
            objResults.Dump();

            if (nearestDealerQuery.HasErrors())
            {
                foreach (GraphQLError gqlErr in nearestDealerQuery.GqlErrors)
                {
                    Console.WriteLine("Error : " + gqlErr.Message);
                    foreach (var loc in gqlErr.Locations)
                    {
                        Console.WriteLine("  -->Location Line : " + loc.Line + ", Column : " + loc.Column);
                    }
                }
            }
            else
            {
                Console.WriteLine("No Errors Found");
            }

            Console.WriteLine("Done with Raw NearestDealer Query Get");

        }
    }
}
