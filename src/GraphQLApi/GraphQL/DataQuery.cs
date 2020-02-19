// GENERATED BY CodeGenerationUtility!!!
using System.Collections.Generic;
using System.Linq;
using DataModel;
using GraphQL;
using GraphQL.Types;
using GraphQLApi.Models;
using GraphQLApi.Types;
using GraphQLApi.Types.Custom;
using GraphQLApi.Types.Custom.Inputs;

namespace GraphQLApi.GraphQL
{
    /*
     * IMPORTANT - This class was generated by the CodeGenerationUtility.
     * To make a change edit the generation templates and run the tool
     * This must be done every time the models are updated,
     * or if the logic needs enhancing...
     */
    public class DataQuery : ObjectGraphType<object>
    {
        private readonly QueryFactoryHelper _queryFactoryHelper;

        public DataQuery(QueryFactoryHelper queryFactoryHelper)
        {
            _queryFactoryHelper = queryFactoryHelper;

            Name = "Query";

            InitQuery<CourseType, Course>();

            InitQuery<DepartmentType, Department>();

            InitQuery<StudentType, Student>();


        }

        private void InitQuery<TFieldType, TModelType>() where TFieldType : IGraphType
        {
            var entityName = typeof(TModelType).Name;

            FieldAsync<ResultType<TFieldType, TModelType>>(
                entityName.ToLowerInvariant(),
                arguments: new QueryArguments(
                    new QueryArgument<ListGraphType<FilterGraphType>>() { Name = "filter", Description = "Filter by any field where field = value" },
                    new QueryArgument<StringGraphType>() { Name = "orderBy", Description = "The name of the field to order by." },
                    new QueryArgument<NonNullGraphType<IntGraphType>>() { Name = "pageSize", Description = "Page size for pagination" },
                    new QueryArgument<IntGraphType>() { Name = "page", Description = "Page for pagination" }
                ),
                resolve: async context =>
                {
                    // get arguments

                    var filters = context.GetArgument<List<Filter>>("filter")?.ToList() ?? new List<Filter>();
                    var orderBy = context.GetArgument<string>("orderBy");

                    // build query
                    var queryFactory = _queryFactoryHelper.GetQueryFactory();
                    var query = queryFactory.Query(entityName);

                    foreach (var filter in filters)
                    {
                        query.Where(filter.Field, filter.Value);
                    }

                    if (!string.IsNullOrEmpty(orderBy))
                    {
                        query.OrderBy(orderBy);
                    }

                    // execute query

                    return await query.UseOffsetPaginationAsync<TModelType>(context);
                }
            );
        }
    }



}

