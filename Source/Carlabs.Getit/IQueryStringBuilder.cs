namespace Carlabs.Getit
{
    public interface IQueryStringBuilder
    {
        /// <summary>
        /// Clear the QueryStringBuilder and all that entails
        /// </summary>
        void Clear();

        /// <summary>
        /// Build the entire query into a string. This will take
        /// the query object and build a graphql query from it. This
        /// returns the query, but not outer block. This is done so
        /// you can use the output to batch the queries
        /// </summary>
        /// <param name="query">The Query</param>
        /// <param name="indent">Indent characters, default = 0</param>
        /// <returns>GraphQL query string wihout outer block</returns>
        string Build(IQuery query, int indent = 0);
    }
}
