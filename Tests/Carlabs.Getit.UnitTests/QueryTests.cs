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

        [TestMethod]
        public void Select_StringList_AddsToQuery()
        {
            // Arrange
            Query query = new Query();
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
            Query query = new Query();
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
            Query query = new Query();
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
            Query query = new Query();

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
            Query query = new Query();
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
            Query query = new Query();
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
            Query query = new Query();

            // Act
            query.Where("id", 1);

            // Assert
            Assert.AreEqual(1, query.WhereMap["id"]);
        }

        [TestMethod]
        public void Where_StringArgumentWhere_AddsToWhere()
        {
            // Arrange
            Query query = new Query();

            // Act
            query.Where("name", "danny");

            // Assert
            Assert.AreEqual("danny", query.WhereMap["name"]);
        }

        [TestMethod]
        public void Where_DictionaryArgumentWhere_AddsToWhere()
        {
            // Arrange
            Query query = new Query();
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
            Query query = new Query();
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
        public void With_Query_AddsToWithList()
        {
            // Arrange
            Query query = new Query();
            Query query2 = new Query();

            // Act
            query.With(query2);

            // Assert
            Assert.AreEqual(query2, query.WithList.First());
        }

        [TestMethod]
        public void With_QueryWithProps_AddsToWithList()
        {
            // Arrange
            Query query = new Query();
            Query query2 = new Query();
            query2.Select("id", "name").From("user").Where("id", 1);

            // Act
            query.Select("price").Where("id", 2).With(query2);

            // Assert
            Assert.AreEqual(1, query.WithList.First().WhereMap["id"]);
            Assert.AreEqual("user", query.WithList.First().Name);
        }
    }
}
