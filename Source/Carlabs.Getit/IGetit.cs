using System;
using System.Threading.Tasks;

namespace Carlabs.Getit
{
    public interface IGetit
    {
        /// <summary>
        /// Holds getit config if used
        /// </summary>
        // ReSharper disable once UnusedMemberInSuper.Global
        IConfig Config { set; get; }

        /// <summary>
        /// The Query Factory/Dispenser
        /// </summary>
        /// <returns></returns>
        IQuery Query();

        /// <summary>
        /// Given a type return the results of a GraphQL query in it. If
        /// the type is a string then will return the JSON string. The resultName
        /// will be automatically set the Name or Alias name if not specified.
        /// For Raw queries you must set the resultName param OR set the Name() in
        /// the query to match.
        /// </summary>
        /// <typeparam name="T">Data Type, typically a list of the record but not always.
        /// </typeparam>
        /// <param name="query">Created Query</param>
        /// <param name="resultName">Overide of the Name/Alias of the query</param>
        /// <returns>The type (T) of object stuffed with data from the query</returns>
        /// <exception cref="ArgumentException">Dupe Key</exception>
        Task<T> Get<T>(IQuery query, string resultName = null);

        /// <summary>
        /// Given a type return the results of a GraphQL query in it. If
        /// the type is a string then will return the JSON string. The resultName
        /// will be automatically set the Name or Alias name if not specified.
        /// For Raw queries you must set the resultName param OR set the Name() in
        /// the query to match.
        /// </summary>
        /// <typeparam name="T">Data Type, typically a list of the record but not always.
        /// </typeparam>
        /// <param name="query">Created Query</param>
        /// <param name="config">Configuration</param>
        /// <param name="resultName">Overide of the Name/Alias of the query</param>
        /// <returns>The type (T) of object stuffed with data from the query</returns>
        /// <exception cref="ArgumentException">Dupe Key</exception>
        Task<T> Get<T>(IQuery query, IConfig config, string resultName = null);
    }
}
