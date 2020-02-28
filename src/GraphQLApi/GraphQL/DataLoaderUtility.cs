using System.Collections.Generic;
using System.Linq;

namespace GraphQLApi.GraphQL
{
    public class DataLoaderUtility
    {
        public static Dictionary<string, string[]> GetWhereIn(IEnumerable<CompositeKey> ids)
        {
            var enumerableIds = ids as CompositeKey[] ?? ids.ToArray();
            var fields = enumerableIds.First()
                .Select(_ => new KeyValuePair<string, string[]>(_.Key, new string[0]))
                .ToDictionary(x => x.Key, x => x.Value);

            var newFields = new Dictionary<string, string[]>();
            foreach (var (key, _) in fields)
            {
                newFields.Add(key, enumerableIds.Select(compositeKey => compositeKey[key]?.ToString()).ToArray());
            }
            return newFields;
        }
    }
}
