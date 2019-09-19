﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Carlabs.Getit.UnitTests
{
    [TestClass]
    public class QueryStringBuilderTests
    {
        private static string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

        [TestMethod]
        public void BuildQueryParam_IntType_ParseInt()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();

            // Act
            string intStr = queryString.BuildQueryParam(123);

            // Assert
            Assert.AreEqual("123", intStr);
        }

        [TestMethod]
        public void BuildQueryParam_QuotedStringType_ParseString()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();

            // Act
            string strStr = queryString.BuildQueryParam("Haystack");

            // Assert
            Assert.AreEqual("\"Haystack\"", strStr);
        }

        [TestMethod]
        public void BuildQueryParam_DoubleType_ParseDouble()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();

            // Act
            const double Value = 1234.5678;
            string doubleStr = queryString.BuildQueryParam(Value);

            // Assert
            Assert.AreEqual(Value.ToString(), doubleStr);
        }

        [TestMethod]
        public void BuildQueryParam_EnumType_ParseEnum()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();
            EnumHelper enumDisabled = new EnumHelper("DISABLED");

            // Act
            string enumStr = queryString.BuildQueryParam(enumDisabled);

            // Assert
            Assert.AreEqual("DISABLED", enumStr);
        }

        [TestMethod]
        public void BuildQueryParam_CustomType_ParseCustom()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();
            const double from = 444.45;
            const double to = 555.45;
            Dictionary<string, object> fromToMap = new Dictionary<string, object>
            {
                {"from", from},
                {"to", to}
            };

            // Act
            string fromToMapStr = queryString.BuildQueryParam(fromToMap);

            // Assert
            Assert.AreEqual($"{{from:{from}, to:{to}}}", fromToMapStr);
        }

        [TestMethod]
        public void BuildQueryParam_ListType_ParseList()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();

            // Act
            List<int> intList = new List<int>(new[] { 43783, 43784, 43145 });
            string intListStr = queryString.BuildQueryParam(intList);

            // Assert
            Assert.AreEqual("[43783, 43784, 43145]", intListStr);
        }

        [TestMethod]
        public void BuildQueryParam_StringListType_ParseStringList()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();

            // Act
            List<string> strList = new List<string>(new[] { "DB7", "DB9", "Vantage" });
            string strListStr = queryString.BuildQueryParam(strList);

            // Assert
            Assert.AreEqual("[\"DB7\", \"DB9\", \"Vantage\"]", strListStr);
        }

        [TestMethod]
        public void BuildQueryParam_DoubleListType_ParseDoubleList()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();

            // Act
            const double float1 = 123.456;
            const double float2 = 78.901;
            List<double> doubleList = new List<double>(new[] { float1, 456, float2 });
            string doubleListStr = queryString.BuildQueryParam(doubleList);

            // Assert
            Assert.AreEqual($"[{float1}, 456, {float2}]", doubleListStr);
        }

        [TestMethod]
        public void BuildQueryParam_EnumListType_ParseEnumList()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();
            EnumHelper enumDisabled = new EnumHelper("DISABLED");
            EnumHelper enumEnabled = new EnumHelper("ENABLED");
            EnumHelper enumHaystack = new EnumHelper("HAYstack");

            // Act
            List<EnumHelper> enumList = new List<EnumHelper>(new[] { enumEnabled, enumDisabled, enumHaystack });
            string enumListStr = queryString.BuildQueryParam(enumList);

            // Assert
            Assert.AreEqual("[ENABLED, DISABLED, HAYstack]", enumListStr);
        }

        [TestMethod]
        public void BuildQueryParam_NestedListType_ParseNestedList()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();
            List<object> objList = new List<object>(new object[] { "aa", "bb", "cc" });
            EnumHelper enumHaystack = new EnumHelper("HAYstack");

            const double from = 444.45;
            const double to = 555.45;
            Dictionary<string, object> fromToMap = new Dictionary<string, object>
            {
                {"from", from},
                {"to", to},
            };

            Dictionary<string, object> nestedListMap = new Dictionary<string, object>
            {
                {"from", 123},
                {"to", 454},
                {"recurse", objList},
                {"map", fromToMap},
                {"name",  enumHaystack}
            };

            // Act
            string nestedListMapStr= queryString.BuildQueryParam(nestedListMap);

            // Assert
            Assert.AreEqual($"{{from:123, to:454, recurse:[\"aa\", \"bb\", \"cc\"], map:{{from:{from}, to:{to}}}, name:HAYstack}}", nestedListMapStr);
        }

        [TestMethod]
        public void Where_QueryString_ParseQueryString()
        {
            // Arrange
            Query query = new Query();

            List<object> objList = new List<object>(new object[] { "aa", "bb", "cc" });
            EnumHelper enumHaystack = new EnumHelper("HAYstack");

            const double from = 444.45;
            const double to = 555.45;
            Dictionary<string, object> fromToMap = new Dictionary<string, object>
            {
                {"from", from},
                {"to", to},
            };

            Dictionary<string, object> nestedListMap = new Dictionary<string, object>
            {
                {"from", 123},
                {"to", 454},
                {"recurse", objList},
                {"map", fromToMap},
                {"name",  enumHaystack}
            };

            query
                .Name("test1")
                .Select("name")
                .Where(nestedListMap);

            IQueryStringBuilder queryString = new QueryStringBuilder();

            // Act
            queryString.AddParams(query);

            string addParamStr = RemoveWhitespace(queryString.QueryString.ToString());

            // Assert
            Assert.AreEqual(RemoveWhitespace($"from:123,to:454,recurse:[\"aa\",\"bb\",\"cc\"],map:{{from:{from},to:{to}}},name:HAYstack"), addParamStr);
        }

        [TestMethod]
        public void Where_ClearQueryString_EmptyQueryString()
        {
            // Arrange
            IQuery query = new Query();

            List<object> objList = new List<object>(new object[] { "aa", "bb", "cc" });
            EnumHelper enumHaystack = new EnumHelper("HAYstack");

            Dictionary<string, object> fromToMap = new Dictionary<string, object>
            {
                {"from", 444.45},
                {"to", 555.45},
            };

            Dictionary<string, object> nestedListMap = new Dictionary<string, object>
            {
                {"from", 123},
                {"to", 454},
                {"recurse", objList},
                {"map", fromToMap},
                {"name",  enumHaystack}
            };

            query
                .Name("test1")
                .Select("name")
                .Where(nestedListMap);

            IQueryStringBuilder queryString = query.Builder;

            queryString.AddParams(query);

            // Act
            queryString.QueryString.Clear();

            // Assert
            Assert.IsTrue(string.IsNullOrEmpty(queryString.QueryString.ToString()));
        }

        [TestMethod]
        public void Select_QueryString_ParseQueryString()
        {
            // Arrange
            IQuery query = new Query();
            IQuery subSelect = new Query();

            EnumHelper gqlEnumEnabled = new EnumHelper().Enum("ENABLED");
            EnumHelper gqlEnumDisabled = new EnumHelper("DISABLED");

            List<object> subSelList = new List<object>(new object[] { "subName", "subMake", "subModel" });

            Dictionary<string, object> mySubDict = new Dictionary<string, object>
            {
                {"subMake", "aston martin"},
                {"subState", "ca"},
                {"subLimit", 1},
                {"__debug", gqlEnumDisabled},
                {"SuperQuerySpeed", gqlEnumEnabled }
            };

            subSelect
                .Select(subSelList)
                .Name("subSelect")
                .Where(mySubDict);

            // create a sub-select too
            List<object> selList = new List<object>(new object[] { "id", subSelect, "name", "make", "model" });

            query
                .Name("test1")
                .Select("more", "things", "in_a_select")
                .Select(selList);

            // Act
            query.Builder.AddFields(query);
            string addParamStr = RemoveWhitespace(query.Builder.QueryString.ToString());

            // Assert
            Assert.AreEqual(RemoveWhitespace("morethingsin_a_selectidsubSelect(subMake:\"aston martin\",subState:\"ca\",subLimit:1,__debug:DISABLED,SuperQuerySpeed:ENABLED){subNamesubMakesubModel}namemakemodel"), addParamStr);
        }

        [TestMethod]
        public void Comment_AddComment_Match()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();

            // Act
            queryString.AddComments("A Simple Comment\nSecond Line");
            string addCommentsStr = RemoveWhitespace(queryString.QueryString.ToString());

            // Assert
            Assert.AreEqual(RemoveWhitespace("#ASimpleComment#SecondLine"), addCommentsStr);
        }

        [TestMethod]
        public void Build_AllElements_StringMatch()
        {
            // Arrange
            Query query = new Query();
            Query subSelect = new Query();

            EnumHelper gqlEnumEnabled = new EnumHelper().Enum("ENABLED");
            EnumHelper gqlEnumDisabled = new EnumHelper("DISABLED");

            List<object> subSelList = new List<object>(new object[] { "subName", "subMake", "subModel" });

            Dictionary<string, object> mySubDict = new Dictionary<string, object>
            {
                {"subMake", "aston martin"},
                {"subState", "ca"},
                {"subLimit", 1},
                {"__debug", gqlEnumDisabled},
                {"SuperQuerySpeed", gqlEnumEnabled }
            };

            subSelect
                .Select(subSelList)
                .Name("subSelect")
                .Where(mySubDict);

            List<object> selList = new List<object>(new object[] { "id", subSelect, "name", "make", "model" });

            query
                .Name("test1")
                .Alias("test1Alias")
                .Select("more", "things", "in_a_select")
                .Select(selList)
                .Comment("A single line Comment");

            IQueryStringBuilder queryString = query.Builder;

            // Act
            string buildStr = RemoveWhitespace(queryString.Build(query));

            // Assert
            Assert.AreEqual(RemoveWhitespace("test1Alias:test1{#AsinglelineCommentmorethingsin_a_selectidsubSelect(subMake:\"aston martin\",subState:\"ca\",subLimit:1,__debug:DISABLED,SuperQuerySpeed:ENABLED){subNamesubMakesubModel}namemakemodel}"), buildStr);
        }
    }
}
