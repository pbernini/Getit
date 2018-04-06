using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Carlabs.Getit.UnitTests
{
    [TestClass]
    public class QueryTests
    {
        /// <summary>
        /// Model used to test data deserialization
        /// </summary>
        //        [QueryName("model")]
        //        class TestModel
        //        {
        //            public string Value;
        //        }

        private static string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

        [TestMethod]
        public void Select_StringList_AddsToQuery()
        {
            // Arrange

            QueryStringBuilder queryString = new QueryStringBuilder();
            Query query = new Query(queryString);

            List<string> selectList = new List<string>()
            {
                "id",
                "name"
            };

            // Act
            query.Select(selectList);

            // Assert
            CollectionAssert.AreEqual(selectList, query.SelectList);
        }

        [TestMethod]
        public void From_String_AddsToQuery()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();
            Query query = new Query(queryString);

            const string from = "user";

            // Act
            query.From(from);

            // Assert
            Assert.AreEqual(from, query.Name);
        }

        [TestMethod]
        public void Select_String_AddsToQuery()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();
            Query query = new Query(queryString);

            const string select = "id";

            // Act
            query.Select(select);

            // Assert
            Assert.AreEqual(select, query.SelectList.First());
        }

        [TestMethod]
        public void Select_DynamicArguments_AddsToQuery()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();
            Query query = new Query(queryString);

            // Act
            query.Select("some", "thing", "else");

            // Assert
            List<string> shouldEqual = new List<string>()
            {
                "some",
                "thing",
                "else"
            };
            CollectionAssert.AreEqual(shouldEqual, query.SelectList);
        }

        [TestMethod]
        public void Select_ArrayOfString_AddsToQuery()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();
            Query query = new Query(queryString);

            string[] selects =
            {
                "id",
                "name"
            };

            // Act
            query.Select(selects);

            // Assert
            List<string> shouldEqual = new List<string>()
            {
                "id",
                "name"
            };
            CollectionAssert.AreEqual(shouldEqual, query.SelectList);
        }

        [TestMethod]
        public void Select_ChainCombinationOfStringAndList_AddsToQuery()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();
            Query query = new Query(queryString);

            const string select = "id";
            List<string> selectList = new List<string>()
            {
                "name",
                "email"
            };
            string[] selectStrings =
            {
                "array",
                "cool"
            };

            // Act
            query
                .Select(select)
                .Select(selectList)
                .Select("some", "thing", "else")
                .Select(selectStrings);

            // Assert
            List<string> shouldEqual = new List<string>()
            {
                "id",
                "name",
                "email",
                "some",
                "thing",
                "else",
                "array",
                "cool"
            };
            CollectionAssert.AreEqual(shouldEqual, query.SelectList);
        }

        [TestMethod]
        public void Where_IntegerArgumentWhere_AddsToWhere()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();
            Query query = new Query(queryString);

            // Act
            query.Where("id", 1);

            // Assert
            Assert.AreEqual(1, query.WhereMap["id"]);
        }

        [TestMethod]
        public void Where_StringArgumentWhere_AddsToWhere()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();
            Query query = new Query(queryString);

            // Act
            query.Where("name", "danny");

            // Assert
            Assert.AreEqual("danny", query.WhereMap["name"]);
        }

        [TestMethod]
        public void Where_DictionaryArgumentWhere_AddsToWhere()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();
            Query query = new Query(queryString);

            Dictionary<string, int> dict = new Dictionary<string, int>()
            {
                {"from", 1},
                {"to", 100}
            };

            // Act
            query.Where("price", dict);

            // Assert
            Dictionary<string, int> queryWhere = (Dictionary<string, int>) query.WhereMap["price"];
            Assert.AreEqual(1, queryWhere["from"]);
            Assert.AreEqual(100, queryWhere["to"]);
            CollectionAssert.AreEqual(dict, (ICollection) query.WhereMap["price"]);
        }

        [TestMethod]
        public void Where_ChainedWhere_AddsToWhere()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();
            Query query = new Query(queryString);

            Dictionary<string, int> dict = new Dictionary<string, int>()
            {
                {"from", 1},
                {"to", 100}
            };

            // Act
            query
                .Where("id", 123)
                .Where("name", "danny")
                .Where("price", dict);

            // Assert
            Dictionary<string, object> shouldPass = new Dictionary<string, object>()
            {
                {"id", 123},
                {"name", "danny"},
                {"price", dict}
            };
            CollectionAssert.AreEqual(shouldPass, query.WhereMap);
        }

        [TestMethod]
        public void Big_Check()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();
            Query query = new Query(queryString);

            QueryStringBuilder subSelectString = new QueryStringBuilder();
            Query subSelect = new Query(subSelectString);

            // set up a couple of ENUMS
            EnumHelper gqlEnumEnabled = new EnumHelper().Enum("ENABLED");
            EnumHelper gqlEnumDisabled = new EnumHelper("DISABLED");

            // set up a subselection list
            List<object> subSelList = new List<object>(new object[] {"subName", "subMake", "subModel"});

            // set up a subselection parameter (where)
            // has simple string, int, and a couple of ENUMs
            Dictionary<string, object> mySubDict = new Dictionary<string, object>
            {
                {"subMake", "honda"},
                {"subState", "ca"},
                {"subLimit", 1},
                {"__debug", gqlEnumDisabled},
                {"SuperQuerySpeed", gqlEnumEnabled}
            };

            // Create a Sub Select Query
            subSelect
                .Select(subSelList)
                .From("subDealer")
                .Where(mySubDict)
                .Comment("SubSelect Below!");

            // Add that subselect to the main select
            List<object> selList = new List<object>(new object[] {"id", subSelect, "name", "make", "model"});

            // List of ints (IDs)
            List<int> trimList = new List<int>(new[] {43783, 43784, 43145});

            // String List
            List<string> modelList = new List<string>(new[] {"DB7", "DB9", "Vantage"});

            // Another List but of Generic Objects that should work as strings
            List<object> recList = new List<object>(new object[] {"aa", "bb", "cc"});

            // try a dict for the typical from to with doubles
            Dictionary<string, object> recMap = new Dictionary<string, object>
            {
                {"from", 444.45},
                {"to", 555.45},
            };

            // try a dict for nested list and dict
            Dictionary<string, object> fromToPrice = new Dictionary<string, object>
            {
                {"from", 123},
                {"to", 454},
                {"recurse", recList},
                {"map", recMap}
            };

            // Even more stuff nested in the params
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

            // Generate the query with an alias and multi-line comment
            query
                .Select(selList)
                .From("Dealer")
                .Alias("myDealerAlias")
                .Where(myDict)
                .Comment("My First F'n GQL Query with geTit\na second line of comments\nand yet another line of comments");

            // Get and pack results
            string packedResults = RemoveWhitespace(query.ToString());
            string packedCheck = RemoveWhitespace(@"
                    myDealerAlias: Dealer(make: ""honda"", state: ""ca"", limit: 2, trims:[43783, 43784, 43145], models:[""DB7"", ""DB9"", ""Vantage""],
                    price:{ from: 123, to: 454, recurse:[""aa"", ""bb"", ""cc""], map: { from: 444.45, to: 555.45} },
                    __debug: ENABLED){
                    # My First F'n GQL Query with geTit
                    # a second line of comments
                    # and yet another line of comments
                    id
                    subDealer(subMake: ""honda"", subState: ""ca"", subLimit: 1, __debug: DISABLED, SuperQuerySpeed: ENABLED){
                        # SubSelect Below!
                        subName
                        subMake
                        subModel
                    }
                    name
                    make
                    model
                }");

            // Best be the same!
            Assert.AreEqual(packedResults, packedCheck);
        }
    }
}
