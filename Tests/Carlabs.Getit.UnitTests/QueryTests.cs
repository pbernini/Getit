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
        public void Check_Required_From()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();
            Query query = new Query(queryString);

            // Act
            query
                .Select("more", "things", "in_a_select");

            // Assert
            Assert.ThrowsException<ArgumentException>(() => query.ToString());
        }

        [TestMethod]
        public void Check_Required_Select()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();
            Query query = new Query(queryString);

            // Act
            query
                .From("something");

            // Assert
            Assert.ThrowsException<ArgumentException>(() => query.ToString());

        }

        [TestMethod] public void Check_Clear()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();
            Query query = new Query(queryString);

            const string expectedSelect = "field";
            const string expectedFrom = "haystack";
            const string expectedAlias = "calhoon";
            const string expectedComment = "this is a comment";

            Dictionary<string, object> expectedWhere = new Dictionary<string, object>()
            {
                {"dog", "cat"},
                {"limit", 3}
            };

            // Act
            query
                .From(expectedFrom)
                .Select(expectedSelect)
                .Alias(expectedAlias)
                .Where(expectedWhere)
                .Comment(expectedComment);

            // Assert to validate stuff has been set first!
            Assert.AreEqual(expectedFrom, query.Name);
            Assert.AreEqual(expectedAlias, query.AliasName);
            CollectionAssert.AreEqual(expectedWhere, query.WhereMap);
            Assert.AreEqual(expectedSelect, query.SelectList.First());

            // Re-act again to clear, this is the actual test...
            query.Clear();

            string emptyStr = string.Empty;
            expectedWhere.Clear();

            // Assert it's all empty
            Assert.AreEqual(emptyStr, query.Name);
            Assert.AreEqual(emptyStr, query.AliasName);
            CollectionAssert.AreEqual(expectedWhere, query.WhereMap);
            Assert.AreEqual(0, query.SelectList.Count());
            Assert.AreEqual(emptyStr, query.QueryComment);
        }

    }
}
