using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using GraphQLApi.Models;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace GraphQLApi.GraphQL
{
    public static class GraphQLQueryExtensions
    {
        public static async Task<Result<TEntity>> UseOffsetPaginationAsync<TEntity>(this Query query,
            IResolveFieldContext<object> context)
        {
            if (!(query is XQuery xQuery)) throw new ArgumentException("Make sure the query object is instantiated from a queryFactory", nameof(query));

            var pageSize = context.GetArgument<int?>("pageSize");
            var page = context.GetArgument<int?>("page") ?? 1;
            var countQuery = xQuery.Clone();

            countQuery
                .Clauses
                .RemoveAll(q => new[] {"select", "order", "skip", "take"}.Contains(q.Component));

            countQuery.SelectRaw("COUNT(*)");

            var totalCount = (await countQuery.GetAsync<int>()).SingleOrDefault();

            if (totalCount == 0) return new Result<TEntity>();

            var mainQuery = xQuery.Clone();
            var hasNextPage = true;
            if (pageSize != null)
            {
                var originalPageSize = pageSize.Value;
                var skip = (page - 1) * pageSize.Value;
                if (totalCount - skip < pageSize)
                {
                    pageSize = totalCount - skip;
                }
                if (pageSize < originalPageSize)
                {
                    hasNextPage = false;
                }
                mainQuery = mainQuery.Take(pageSize.Value).Skip(skip);
            }

            var data = mainQuery.Get<TEntity>().ToArray();

            if (!data.Any())
            {
                return new Result<TEntity>()
                {
                    TotalCount = 0
                };
            }

            return new Result<TEntity>()
            {
                TotalCount = totalCount,
                Items = data,
                PageInfo = new PageInfo()
                {
                    HasNextPage = hasNextPage,
                    HasPreviousPage = true
                }
            };
        }
    }
}
