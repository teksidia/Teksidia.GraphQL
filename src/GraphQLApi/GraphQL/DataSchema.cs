using System;
using GraphQL.Types;
using GraphQL.Utilities;

namespace GraphQLApi.GraphQL
{
    public class DataSchema : Schema
    {
        public DataSchema(IServiceProvider provider)
            : base(provider)
        {
            Query = provider.GetRequiredService<DataQuery>();
        }
    }
}
