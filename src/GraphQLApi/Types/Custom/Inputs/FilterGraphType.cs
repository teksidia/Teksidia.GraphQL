using GraphQL.Types;
using GraphQLApi.Models;

namespace GraphQLApi.Types.Custom.Inputs
{
    public sealed class FilterGraphType : InputObjectGraphType<Filter>
    {
        public FilterGraphType()
        {
            Name = "Filter";
            Description = "Allows filtering on a field. `field` is the name (e.g. MmuId) and `value` is the required value.";

            Field(input => input.Field);
            Field(input => input.Value);
        }
    }
}
