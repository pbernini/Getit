
using System;
using System.Collections.Generic;
using Carlabs.Getit;


namespace Carlabs.Examples.Getit
{
    class Program
    {
        // ReSharper disable once UnusedParameter.Local
        static void Main(string[] args)
        {
            QueryStringBuilder queryString = new QueryStringBuilder();
            Query query = new Query(queryString);

            QueryStringBuilder subSelectString = new QueryStringBuilder();
            Query subSelect = new Query(subSelectString);
            // selection of fields to return, not compound sub-selections, only simple types

            EnumHelper gqlEnumEnabled = new EnumHelper().Enum("ENABLED");
            EnumHelper gqlEnumDisabled = new EnumHelper("DISABLED");
            gqlEnumDisabled.Enum("SOMETHNG_ENUM");

            List<object> subSelList = new List<object>(new object[] { "subName", "subMake", "subModel" });

            Dictionary<string, object> mySubDict = new Dictionary<string, object>
            {
                {"subMake", "honda"},
                {"subState", "ca"},
                {"subLimit", 1},
                {"__debug", gqlEnumDisabled},
                {"SuperQuerySpeed", gqlEnumEnabled }
            };

            subSelect
                .Select(subSelList)
                .From("subDealer")
                .Where(mySubDict)
                .Comment("SubSelect Below!");

            //List<string> selList = new List<string>(new string[] {"id", "name", "make", "model"});
            List<object> selList = new List<object>(new object[] { "id", subSelect, "name", "make", "model" });

            // try list of ints and list of strings
            List<int> trimList = new List<int>(new [] {43783, 43784, 43145});
            List<string> modelList = new List<string>(new [] { "DB7", "DB9", "Vantage" });
            List<object> recList = new List<object>(new object[] { "aa", "bb", "cc" });

            // try a dict for the typical from to
            Dictionary<string, object> recMap = new Dictionary<string, object>
            {
                {"from", 444.45},
                {"to", 555.45},
            };

            // try a dict for the typical from to
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

            query
                .Select(selList)
                .From("Dealer")
                .Alias("myDealerAlias")
                .Where(myDict)
                .Comment("My First F'n GQL Query with geTit\na second line of comments\nand yet another line of comments");

            Console.WriteLine($"{query}");
            subSelect.Alias("myDealerSubSelect");
            Console.WriteLine($"{subSelect}");
        }
    }
}
