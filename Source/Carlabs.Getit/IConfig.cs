using System;

namespace Carlabs.Getit
{
    public interface IConfig
    {
        /// <summary>
        /// The URL of the GraphQL service.
        /// </summary>
        string Url { get; }

        /// <summary>
        /// Sets the config URL.
        /// If empty or null it'll throw an exception.
        /// </summary>
        /// <param name="url"></param>
        /// <exception cref="ArgumentException">Thrown when url is empty or null</exception>
        void SetUrl(string url);
    }
}
