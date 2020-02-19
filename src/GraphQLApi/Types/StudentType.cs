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
    public sealed partial class StudentType : ObjectGraphType<Student>
    {
        public StudentType(QueryFactoryHelper queryFactoryHelper, IDataLoaderContextAccessor dataLoaderContextAccessor)
        {
            Name = "Student";

            Field(h => h.Id).Description("Id");

            Field(h => h.Surname, true).Description("Surname");

            Field(h => h.Forename, true).Description("Forename");



            ChildFields(queryFactoryHelper, dataLoaderContextAccessor);

        }
    }
}