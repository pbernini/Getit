using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Carlabs.Getit
{
    public class QueryStringBuilder
    {
        private StringBuilder QueryString;
        const int IndentSize = 4;

        public QueryStringBuilder()
        {
            QueryString = new StringBuilder();
        }

        private string BuildQueryParam(object value)
        {
            // Nicely use the pattern match

            switch (value)
            {
                case string strValue:
                    return "\"" + strValue + "\"";

                case int intValue:
                    return intValue.ToString();

                case float floatValue:
                    return floatValue.ToString(CultureInfo.CurrentCulture);

                case double doubleValue:
                    return doubleValue.ToString(CultureInfo.CurrentCulture);

                case EnumHelper enumValue:
                    return enumValue.ToString();

                case KeyValuePair<string, object> kvValue:
                    StringBuilder keyValueStr = new StringBuilder();

                    keyValueStr.Append($"{kvValue.Key}:{BuildQueryParam(kvValue.Value)}");
                    return keyValueStr.ToString();

                case IList listValue:
                    StringBuilder listStr = new StringBuilder();

                    listStr.Append("[");
                    foreach (var obj in listValue)
                    {
                        listStr.Append(BuildQueryParam(obj) + ", ");
                    }

                    // strip comma-space from local list

                    listStr.Length--;
                    listStr.Length--;
                    listStr.Append("]");

                    return listStr.ToString();

                case IDictionary dictValue:
                    StringBuilder dictStr = new StringBuilder();

                    dictStr.Append("{");
                    foreach (var dictObj in (Dictionary<string, object>) dictValue)
                    {
                        dictStr.Append(BuildQueryParam(dictObj) + ", ");
                    }

                    // strip comma-space from local list

                    dictStr.Length--;
                    dictStr.Length--;
                    dictStr.Append("}");

                    return dictStr.ToString();

                default:
                    throw new InvalidDataException("Unsupported Query Parameter, Type Found : " + value.GetType());
            }
        }

        private void AddParams(Query query)
        {
            // Build the param list from the name value pairs.
            // All entries have a `name`:`value` looking format. The
            // BuildQueryParam's will recurse any nested data elements

            foreach (var param in query.WhereMap)
            {
                QueryString.Append($"{param.Key}:");
                QueryString.Append(BuildQueryParam(param.Value) + ", ");
            }

            // Remove the last comma and space that always trails!
            QueryString.Length--;
            QueryString.Length--;
        }

        private void AddFields(Query query, int indent = 0)
        {
            // Build the param list from the name value pairs. NOTE
            // This will build array or objects differently based on the
            // type of the value object 

            string strPad = new String(' ', indent);

            foreach (var field in query.SelectList)
            {
                if (field is string)
                {
                    QueryString.Append(strPad + $"{field}\n");
                }
                else
                    if (field  is Query)
                    {
                        QueryStringBuilder subQuery = new QueryStringBuilder();
                        QueryString.Append($"{subQuery.Build((Query)field, indent)}\n");
                    }
                    else
                    {
                        throw new ArgumentException("Invalid Field Type Specified, must be `string` or `Query`");
                    }
            }
        }

        private void AddComments(string comments, int indent)
        {
            string pad = new String(' ', indent);
            string comment = comments.Replace("\n", $"\n{pad}# ");
            QueryString.Append(pad + "# " + comment + "\n");
        }

        public string Build(Query query, int indent = 0)
        {
            // build the comment string, new lines get the #

            string pad = new String(' ', indent);
            string prevPad = pad;

            // if we have an alias add it before the name, no padding for next field which is the name!

            if (!String.IsNullOrWhiteSpace(query.AliasName))
            {
                QueryString.Append(pad + $"{query.AliasName}:");
                pad = "";
            }

            // here we go, start with the name

            QueryString.Append(pad + query.Name + "(");

            // for all Params (these are the where parts) get and build param list
            // params don't get padded they are all on one line typically

            AddParams(query);
            QueryString.Append(")");

            // now build the Field list, bump padding as we are inside a query (field block)

            indent += IndentSize;

            QueryString.Append("{\n");

            // Stuff any comments in the field (select) section of the query

            AddComments(query.GqlComment, indent);
            AddFields(query, indent);

            QueryString.Append(prevPad + "}");

            return QueryString.ToString();

            //throw new NotImplementedException();
        }
    }
}
