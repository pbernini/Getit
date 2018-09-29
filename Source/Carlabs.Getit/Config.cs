using System;
using EnsureThat;

namespace Carlabs.Getit
{
    public class Config : IConfig
    {
        public Config()
        {
        }

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
            Ensure.String.IsNotNullOrWhiteSpace(
                url,
                nameof(url),
                opts => opts.WithMessage("GraphQL Url can't be blank or null.")
            );
            Url = url;
        }

    }
}
