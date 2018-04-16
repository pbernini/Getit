using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

//using Carlabs.Rlp.Clients;
//using Carlabs.Getit.Containers;
//using Carlabs.Rlp.Http;

namespace Carlabs.Getit.AutoFac
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

