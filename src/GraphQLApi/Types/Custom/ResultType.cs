using GraphQL.Types;
using GraphQLApi.Models;

namespace GraphQLApi.Types.Custom
{
    public sealed class ResultType<TGraphType, TEntity> : ObjectGraphType<Result<TEntity>> where TGraphType : IGraphType
    {
        private readonly string _name = typeof(TEntity).Name;

        public ResultType()
        {
            Name = $"{_name}Result";
            Description = $"A wrapper for `{_name}` result set.";

            Field(input => input.TotalCount);

            Field<PageInfoType>("pageInfo",
                    "Information to aid in pagination.",
                    resolve: context => context.Source.PageInfo);

            Field<ListGraphType<TGraphType>>("items",
                 $"A list of all of the `{_name}`s returned.",
                 resolve: context => context.Source.Items);
        }
    }
}
