// GENERATED BY CodeGenerationUtility!!!
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.DataLoader;
using GraphQL.Types;
using DataModel;
using GraphQLApi.GraphQL;
using SqlKata.Execution;

namespace GraphQLApi.Types
{
    /*
     * IMPORTANT - This class was generated by the CodeGenerationUtility.
     * To make a change edit the generation templates and run the tool
     * This must be done every time the models are updated,
     * or if the logic needs enhancing...
     */
    public sealed partial class StudentType : ObjectGraphType<Student>
    {
        public StudentType(QueryFactoryHelper queryFactoryHelper, IDataLoaderContextAccessor dataLoaderContextAccessor)
        {
            Name = "Student";

            Field(h => h.Id).Description("Id");

            Field(h => h.Surname, true).Description("Surname");

            Field(h => h.Forename, true).Description("Forename");

            Field(h => h.CourseId).Description("CourseId");

              Field<CourseType, Course>()
                .Name("course")
                .Description("Related course")
                .ResolveAsync(context =>
                {

                    Func<IEnumerable<CompositeKey>, CancellationToken, Task<IDictionary<CompositeKey, Course>>> batchLoaderLogic = async (ids, cancelToken) =>
                    {
                        var queryFactory = queryFactoryHelper.GetQueryFactory();

                        var query = queryFactory
                            .Query("Course");

                        var whereIn = DataLoaderUtility.GetWhereIn(ids);

                        foreach (var (field, fieldIds) in whereIn)
                        {
                            query.WhereIn(field, fieldIds);
                        }

                        var result = await query.GetAsync<Course>();

                        var dict = new Dictionary<CompositeKey, Course>(new CompositeKey.MyEqualityComparer());
                        foreach (var r in result)
                        {
                            var fkFields = new Dictionary<string, object>()
                                {
                                    {"CourseId", r.CourseId}
                                };
                            var key = new CompositeKey(fkFields);
                            dict.TryAdd(key, r);
                        }

                        return dict;
                    };

                    var dataLoader = dataLoaderContextAccessor.Context.GetOrAddBatchLoader("Student_Course", batchLoaderLogic);
                    var parentFields = new Dictionary<string, object>()
                    {
                        {"CourseId", context.Source.CourseId}
                    };
                    return dataLoader.LoadAsync(new CompositeKey(parentFields));

                });



        }
    }
}
