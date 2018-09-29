namespace Carlabs.Getit
{
    public class Getit : IGetit
    {
        /// <summary>
        /// Wrapper class that dispenses Query's. Each
        /// Query needs a new StringBuilder and a Set Up config
        /// which has a URL set.
        /// </summary>
        /// <returns>IQuery</returns>
        public IQuery Query(IConfig config)
        {
            return new Query(new QueryStringBuilder(), config);
        }
    }
}
