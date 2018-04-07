using System.Collections.Generic;

namespace Carlabs.Getit
{
    public interface IQuery
    {
        List<object> SelectList { get; }
        Dictionary<string, object> WhereMap { get; }
        string Name { get; }
        string AliasName { get; }
        string QueryComment { get; }

        /// <summary>
        /// Sets the query Name
        /// </summary>
        /// <param name="from">The Query Name String</param>
        /// <returns>Query</returns>
        Query From(string from);

        /// <summary>
        /// Sets the Query Alias name. This is used in graphQL to allow 
        /// multipe queries with the same endpoint (name) to be assembled
        /// into a batch like query. This will prefix the From Name as specified.
        /// Note that this can be applied to any sub-select as well. GraphQL will
        /// rename the query with the alias name in the response.
        /// </summary>
        /// <param name="alias">The alias name</param>
        /// <returns>Query</returns>
        Query Alias(string alias);

        /// <summary>
        /// Add a comment to the Query. This will take a simple string comment
        /// and add it to the Select block in the query. GraphQL formatting will
        /// be automatically added. Multi-line comments can be done with the
        /// '\n' character and it will be automatically converted into a GraphQL
        /// multi-line coment
        /// </summary>
        /// <param name="comment">The comment string</param>
        /// <returns>Query</returns>
        Query Comment(string comment);

        /// <summary>
        /// Add this list to the select part of the query. This
        /// accepts any type of list, but must be one of the types
        /// of data we support, primitives, lists, maps
        /// </summary>
        /// <param name="objectList">Generic List of select fields</param>
        /// <returns>Query</returns>
        Query Select(IEnumerable<object> objectList);

        /// <summary>
        /// Add a list of simple strings to the selection part of the 
        /// query.
        /// </summary>
        /// <param name="selects">List of strings</param>
        /// <returns>Query</returns>
        Query Select(params string[] selects);

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
        Query Where(string key, object where);

        /// <summary>
        /// Add a dict of key value pairs &lt;string, object&gt; to the existing where part
        /// </summary>
        /// <param name="dict">An existing Dictionay that takes &lt;string, object&gt;</param>
        /// <returns>Query</returns>
        /// <throws>Dupekey and others</throws>
        Query Where(Dictionary<string, object> dict);

        T Get<T>();

        /// <summary>
        /// Gets the string representation of the GraphQL query. This does some
        /// MINOR checking and will toss if not formatted correctly
        /// </summary>
        /// <returns>The GrapQL Query String, without outer enclosing block</returns>
        /// <throws>ArgumentException</throws>
        string ToString();
    }
}
