using System;

namespace Carlabs.Getit
{
    public class Config : IConfig
    {
        /// <summary>
        /// Default constructor, don't forget to set up
        /// any need options
        /// </summary>
        public Config()
        {
        }

        /// <summary>
        /// Construct the config with the URL of the GraphQL
        /// service.
        /// </summary>
        /// <param name="url"></param>
        public Config(string url)
        {
            SetUrl(url);
        }

        /// <summary>
        /// The URL of the GraphQL service.
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Sets the config URL.
        /// If empty or null it'll throw an exception.
        /// </summary>
        /// <param name="url"></param>
        /// <exception cref="ArgumentException">Thrown when url is empty or null</exception>
        public void SetUrl(string url)
        {
            if (String.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("GraphQL `Url` can't be blank or null.");
            }

            Url = url;
        }

    }
}
