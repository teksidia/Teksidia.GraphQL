using System.Collections.Generic;
using System.Text;

namespace GraphQLApi.GraphQL
{
    public class CompositeKey : Dictionary<string, object>
    {
        private readonly string _internalKey;

        public CompositeKey(IDictionary<string, object> dictionary) : base(dictionary)
        {
            var sb = new StringBuilder();
            foreach (var (key, value) in this)
            {
                sb.Append($"{key}={value}|");
            }
            _internalKey = sb.ToString();
        }

        public override string ToString()
        {
            return _internalKey;
        }

        public class MyEqualityComparer : IEqualityComparer<CompositeKey>
        {
            public bool Equals(CompositeKey x, CompositeKey y)
            {
                return x?.ToString() == y?.ToString();
            }

            public int GetHashCode(CompositeKey x)
            {
                unchecked
                {
                    var hash = 17;
                    foreach (var (_, item) in x)
                    {
                        hash = hash * 23 + (item ?? "").GetHashCode();
                    }
                    return hash;
                }
            }

        }
    }
}
