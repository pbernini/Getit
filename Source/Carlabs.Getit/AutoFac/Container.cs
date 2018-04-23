using Autofac;

namespace Carlabs.Getit.Autofac
{
    public static class Container
    {
        /// <summary>
        /// The singelton container
        /// </summary>
        private static IContainer _container;

        /// <summary>
        /// Checks to see if container is initialized
        /// </summary>
        private static bool IsInitialized => _container != null;

        /// <summary>
        /// Initializes autofac container.
        /// This is also used to register all needed depedencies.
        /// </summary>
        private static void Initialize()
        {
            ContainerBuilder containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterType<Config>().As<IConfig>().SingleInstance();
            containerBuilder.RegisterType<Query>().As<IQuery>().InstancePerDependency();
            containerBuilder.RegisterType<QueryStringBuilder>().As<IQueryStringBuilder>().InstancePerDependency();

            _container = containerBuilder.Build();
        }

        /// <summary>
        /// Resolve interface
        /// </summary>
        /// <typeparam name="T">The element type of interface being resolved</typeparam>
        /// <returns>Type registered to resolve from interface</returns>
        public static T Resolve<T>()
        {
            if (!IsInitialized)
            {
                Initialize();
            }

            return _container.Resolve<T>();
        }
    }
}

