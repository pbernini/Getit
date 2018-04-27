using System;
using Carlabs.Getit.Autofac;

namespace Carlabs.Getit
{
    public static class Getit
    {
        /// <summary>
        /// The <c>Config</c> class used to set and get configuration values.
        /// </summary>
        /// <example>
        /// <code>
        /// // Set the Config URL to the GraphQL endpoint
        /// Getit.Config.SetUrl("http://example.com/graphql/endpoint");
        /// </code>
        /// </example>
        public static IConfig Config => Container.Resolve<IConfig>();

        /// <summary>
        /// Get's a new query instances. Must not
        /// be done in the constructor since that only
        /// gets called one time. This will dispence a 
        /// new IQuery fresh each time
        /// </summary>
        /// <returns>IQuery</returns>
        public static IQuery Query => Container.Resolve<IQuery>();
    }
}
