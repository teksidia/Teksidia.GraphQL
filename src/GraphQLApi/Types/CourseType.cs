// GENERATED BY CodeGenerationUtility!!!
using DataModel;
using GraphQL.DataLoader;
using GraphQL.Types;

namespace GraphQLApi.Types
{
    /*
     * IMPORTANT - This class was generated by the CodeGenerationUtility.
     * To make a change edit the generation templates and run the tool
     * This must be done every time the models are updated,
     * or if the logic needs enhancing...
     */
    public sealed partial class CourseType : ObjectGraphType<Course>
    {
        public CourseType(QueryFactoryHelper queryFactoryHelper, IDataLoaderContextAccessor dataLoaderContextAccessor)
        {
            Name = "Course";

            Field(h => h.CourseId).Description("CourseId");

            Field(h => h.Name, true).Description("Name");

            Field(h => h.DepartmentId, true).Description("DepartmentId");

            Field(h => h.Credits).Description("Credits");



            ChildFields(queryFactoryHelper, dataLoaderContextAccessor);

        }
    }
}