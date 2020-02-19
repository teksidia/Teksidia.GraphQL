using GraphQL.Types;
using GraphQLApi.Models;

namespace GraphQLApi.Types.Custom
{
    public sealed class PageInfoType : ObjectGraphType<PageInfo>
    {
        public PageInfoType()
        {
            Name = "PageInfo";
            Description = "Information to aid in pagination.";

            Field(input => input.HasNextPage);
            Field(input => input.HasPreviousPage);
        }
    }
}
