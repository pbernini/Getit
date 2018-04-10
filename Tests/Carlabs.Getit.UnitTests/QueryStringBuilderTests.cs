using System;
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
        public void Check_Int_Type_Value()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();

            // Act
            string intStr = queryString.BuildQueryParam(123);

            // Assert
            Assert.AreEqual("123", intStr);
        }

        [TestMethod]
        public void Check_String_Type_Value()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();

            // Act
            string strStr = queryString.BuildQueryParam("Haystack");

            // Assert
            Assert.AreEqual("\"Haystack\"", strStr);
        }

        [TestMethod]
        public void Check_Double_Type_Value()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();

            // Act
            string doubleStr = queryString.BuildQueryParam(1234.5678);

            // Assert
            Assert.AreEqual("1234.5678", doubleStr);
        }

        [TestMethod]
        public void Check_Enum_Type_Value()
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
        public void Check_Custom_Type_Values()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();
            Dictionary<string, object> fromToMap = new Dictionary<string, object>
            {
                {"from", 444.45},
                {"to", 555.45}
            };

            // Act
            string fromToMapStr = queryString.BuildQueryParam(fromToMap);

            // Assert
            Assert.AreEqual("{from:444.45, to:555.45}", fromToMapStr);
        }

        [TestMethod]
        public void Check_Int_List_Type_Values()
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
        public void Check_String_List_Type_Values()
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
        public void Check_Double_List_Type_Values()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();

            // Act
            List<double> doubleList = new List<double>(new[] { 123.456, 456, 78.901 });
            string doubleListStr = queryString.BuildQueryParam(doubleList);

            // Assert
            Assert.AreEqual("[123.456, 456, 78.901]", doubleListStr);
        }

        [TestMethod]
        public void Check_Enum_List_Type_Values()
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
        public void Check_Custom_Nested_Type_Values()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();
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

            // Act
            string nestedListMapStr= queryString.BuildQueryParam(nestedListMap);

            // Assert
            Assert.AreEqual("{from:123, to:454, recurse:[\"aa\", \"bb\", \"cc\"], map:{from:444.45, to:555.45}, name:HAYstack}", nestedListMapStr);
        }

        [TestMethod]
        public void Check_Add_Params()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();
            Query query = new Query(queryString);

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
                .From("test1")
                .Select("name")
                .Where(nestedListMap);

            // Act
            queryString.AddParams(query);
            string addParamStr = RemoveWhitespace(queryString.QueryString.ToString());

            // Assert
            Assert.AreEqual(RemoveWhitespace("from:123,to:454,recurse:[\"aa\",\"bb\",\"cc\"],map:{from:444.45,to:555.45},name:HAYstack"), addParamStr);
        }

        [TestMethod]
        public void Check_Add_Fields()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();
            Query query = new Query(queryString);
            QueryStringBuilder subSelectString = new QueryStringBuilder();
            Query subSelect = new Query(subSelectString);
            EnumHelper gqlEnumEnabled = new EnumHelper().Enum("ENABLED");
            EnumHelper gqlEnumDisabled = new EnumHelper("DISABLED");

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
                .From("subSelect")
                .Where(mySubDict);

            List<object> selList = new List<object>(new object[] { "id", subSelect, "name", "make", "model" });

            query
                .From("test1")
                .Select("more", "things", "in_a_select")
                .Select(selList);

            // Act
            queryString.AddFields(query);
            string addParamStr = RemoveWhitespace(queryString.QueryString.ToString());

            // Assert
            Assert.AreEqual(RemoveWhitespace("morethingsin_a_selectidsubSelect(subMake:\"honda\",subState:\"ca\",subLimit:1,__debug:DISABLED,SuperQuerySpeed:ENABLED){subNamesubMakesubModel}namemakemodel"), addParamStr);
        }

        [TestMethod]
        public void Check_Add_Comment()
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
        public void Check_Build()
        {
            // Arrange
            QueryStringBuilder queryString = new QueryStringBuilder();
            Query query = new Query(queryString);
            QueryStringBuilder subSelectString = new QueryStringBuilder();
            Query subSelect = new Query(subSelectString);
            EnumHelper gqlEnumEnabled = new EnumHelper().Enum("ENABLED");
            EnumHelper gqlEnumDisabled = new EnumHelper("DISABLED");

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
                .From("subSelect")
                .Where(mySubDict);

            List<object> selList = new List<object>(new object[] { "id", subSelect, "name", "make", "model" });

            query
                .From("test1")
                .Alias("test1Alias")
                .Select("more", "things", "in_a_select")
                .Select(selList)
                .Comment("A single line Comment");

            // Act
            string buildStr = RemoveWhitespace(queryString.Build(query));

            // Assert
            Assert.AreEqual(RemoveWhitespace("test1Alias:test1{#AsinglelineCommentmorethingsin_a_selectidsubSelect(subMake:\"honda\",subState:\"ca\",subLimit:1,__debug:DISABLED,SuperQuerySpeed:ENABLED){subNamesubMakesubModel}namemakemodel}"), buildStr);
        }
    }
}
