using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlKata.Execution;

namespace GraphQLApi.GraphQL
{
    public static class ChildQueryHelper
    {
        public static async Task<IEnumerable<TEntity>> Get<TEntity>(string tableName, QueryFactoryHelper queryFactoryHelper, IEnumerable<CompositeKey> ids)
        {
            var queryFactory = queryFactoryHelper.GetQueryFactory();

            var whereIn = DataLoaderUtility.GetWhereIn(ids);

            const float maximumIdCount = 1000;
            var idCount = whereIn.First().Value.Length;
            var aggregatedResults = new List<TEntity>();

            var totalPages = Math.Ceiling(idCount / maximumIdCount);

            for (var page = 0; page < totalPages; page++)
            {
                var batchedCompositeIds = new Dictionary<string, string[]>();

                foreach (var (key, value) in whereIn)
                {
                    batchedCompositeIds.Add(key,
                        value.Skip((int)(page * maximumIdCount)).Take((int)maximumIdCount).ToArray());
                }

                var query = queryFactory
                    .Query(tableName);

                foreach (var (field, fieldIds) in batchedCompositeIds)
                {
                    query.WhereIn(field, fieldIds);
                }
                var results = await query.GetAsync<TEntity>();
                aggregatedResults.AddRange(results);
            }

            return aggregatedResults;
        }
    }
}
