using System;
using System.Collections.Generic;

namespace Carlabs.Getit
{
    public class Query
    {
        public List<object> SelectList { get; } = new List<object>();
     
        public Dictionary<string, object> WhereMap { get; } = new Dictionary<string, object>();
        public string Name { get; private set; }
        public string AliasName { get; private set; }
        public string GqlComment { get; private set; }

        private readonly QueryStringBuilder _builder;

        public Query(QueryStringBuilder builder)
        {
            _builder = builder;
        }

        public Query(string from, QueryStringBuilder builder)
        {
            Name = from;
            _builder = builder;
        }

        public Query From(string from)
        {
            Name = from;

            return this;
        }

        public Query Alias(string alias)
        {
            AliasName = alias;

            return this;
        }

        public Query Comment(string comment)
        {
            GqlComment = comment;

            return this;
        }

        public Query Select(IEnumerable<object> objectList)
        {
            SelectList.AddRange(objectList);

            return this;
        }

        public Query Select(params string[] selects)
        {
            SelectList.AddRange(selects);

            return this;
        }
        
        public Query Where(string key, object where)
        {
            WhereMap.Add(key, where);

            return this;
        }

        /// <summary>
        /// Add a dict of key value pairs &lt;string, object&gt; to the existing where part
        /// </summary>
        /// <param name="dict">An existing Dictionay that takes &lt;string, object&gt;</param>
        /// <returns>Query</returns>
        /// <throws>Dupekey and others</throws>
        public Query Where(Dictionary<string, object> dict)
        {
            foreach (var field in dict)
                WhereMap.Add(field.Key, field.Value);

            return this;
        }

        public T Get<T>()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return _builder.Build(this);
        }
    }
}
