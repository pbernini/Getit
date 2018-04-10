using System;
using System.Collections.Generic;

namespace Carlabs.Getit
{
    /// <summary>
    /// The Query Class is a simple class to build out graphQL
    /// style queries. It will build the parameters and field lists
    /// similar in a way you would use a SQL query builder to assemble
    /// a query. This is specific
    /// 
    /// 
    /// </summary>
    public class Query : IQuery
    {
        public List<object> SelectList { get; } = new List<object>();
        public Dictionary<string, object> WhereMap { get; } = new Dictionary<string, object>();
        public string Name { get; private set; }
        public string AliasName { get; private set; }
        public string QueryComment { get; private set; }

        private readonly QueryStringBuilder _builder;

        /// <summary>
        /// Constructor needing a QueryStringBuilder to
        /// Hold the results. 
        /// </summary>
        /// <param name="builder">The QueryStringBuilder to use to build it</param>
        public Query(QueryStringBuilder builder)
        {
            _builder = builder;
        }

        /// <summary>
        /// Constructor needing a 'from' (the name) and a QueryStringBuilder to
        /// Hold the results. 
        /// </summary>
        /// <param name="from">The graphQL query name</param>
        /// <param name="builder">The QueryStringBuilder to use to build it</param>
        public Query(string from, QueryStringBuilder builder)
        {
            Name = from;
            _builder = builder;
        }

        /// <summary>
        /// Clear the Query and anything related
        /// </summary>
        public void Clear()
        {
            // reset all member vars to clean state

            _builder.Clear();
            SelectList.Clear();
            WhereMap.Clear();
            Name = string.Empty;
            AliasName = string.Empty;
            QueryComment = string.Empty;
        }

        /// <summary>
        /// Sets the query Name
        /// </summary>
        /// <param name="from">The Query Name String</param>
        /// <returns>Query</returns>
        public Query From(string from)
        {
            Name = from;

            return this;
        }

        /// <summary>
        /// Sets the Query Alias name. This is used in graphQL to allow 
        /// multipe queries with the same endpoint (name) to be assembled
        /// into a batch like query. This will prefix the From Name as specified.
        /// Note that this can be applied to any sub-select as well. GraphQL will
        /// rename the query with the alias name in the response.
        /// </summary>
        /// <param name="alias">The alias name</param>
        /// <returns>Query</returns>
        public Query Alias(string alias)
        {
            AliasName = alias;

            return this;
        }

        /// <summary>
        /// Add a comment to the Query. This will take a simple string comment
        /// and add it to the Select block in the query. GraphQL formatting will
        /// be automatically added. Multi-line comments can be done with the
        /// '\n' character and it will be automatically converted into a GraphQL
        /// multi-line coment
        /// </summary>
        /// <param name="comment">The comment string</param>
        /// <returns>Query</returns>
        public Query Comment(string comment)
        {
            QueryComment = comment;

            return this;
        }

        /// <summary>
        /// Add this list to the select part of the query. This
        /// accepts any type of list, but must be one of the types
        /// of data we support, primitives, lists, maps
        /// </summary>
        /// <param name="objectList">Generic List of select fields</param>
        /// <returns>Query</returns>
        public Query Select(IEnumerable<object> objectList)
        {
            SelectList.AddRange(objectList);

            return this;
        }

        /// <summary>
        /// Add a list of simple strings to the selection part of the 
        /// query.
        /// </summary>
        /// <param name="selects">List of strings</param>
        /// <returns>Query</returns>
        public Query Select(params string[] selects)
        {
            SelectList.AddRange(selects);

            return this;
        }

        /// <summary>
        /// Adds a sub query to the list
        /// </summary>
        /// <param name="subSelect">A sub-selection, which can be just a query</param>
        /// <returns>Query</returns>
        /// <exception cref="ArgumentException"></exception>
        public Query Select(Query subSelect)
        {
            if (String.IsNullOrWhiteSpace(subSelect.Name))
            {
                throw new ArgumentException("Hey silly, sub-selections must have a `From` attribute set");
            }

            SelectList.Add(subSelect);

            return this;
        }

        /// <summary>
        /// Sets up the Parameters part of the GraphQL query. This
        /// accepts a key and a where part that will go into the  
        /// list for later construction into the query. The where part
        /// can be a simple primitive or complex object that will be 
        /// evaluated.
        /// </summary>
        /// <param name="key">The Parameter Name</param>
        /// <param name="where">The value of the parameter, primitive or object</param>
        /// <returns></returns>
        public Query Where(string key, object where)
        {
            WhereMap.Add(key, where);

            return this;
        }

        /// <summary>
        /// Add a dict of key value pairs &lt;string, object&gt; to the existing where part
        /// </summary>
        /// <param name="dict">An existing Dictionay that takes &lt;string, object&gt;</param>
        /// <returns>Query</returns>
        /// <exception cref="ArgumentException">Dupe Key</exception>
        /// <exception cref="ArgumentNullException">Null Argument</exception>
        public Query Where(Dictionary<string, object> dict)
        {
            foreach (KeyValuePair<string, object> field in dict)
                WhereMap.Add(field.Key, field.Value);

            return this;
        }

        public T Get<T>()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the string representation of the GraphQL query. This does some
        /// MINOR checking and will toss if not formatted correctly
        /// </summary>
        /// <returns>The GrapQL Query String, without outer enclosing block</returns>
        /// <exception cref="ArgumentException">Dupe Key</exception>
        public override string ToString()
        {
            // Must have a name 
            if(String.IsNullOrWhiteSpace(Name))
                throw new ArgumentException("Must have a `From` name specified in the Query");

            // and also a select list
            if(SelectList.Count == 0)
                throw new ArgumentException("Must have a one or more `Select` fields in the Query");

            return _builder.Build(this);
        }
    }
}
