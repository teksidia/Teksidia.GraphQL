// GENERATED BY CodeGenerationUtility!!!
using System;
using GraphQLApi.Types;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQLApi
{
    /*
     * IMPORTANT - This class was generated by the CodeGenerationUtility.
     * To make a change edit the generation templates and run the tool
     * This must be done every time the models are updated,
     * or if the logic needs enhancing...
     */
    public partial class Startup
    {
        private void InitGraphTypes(IServiceCollection services)
        {
             services.AddSingleton<CourseType>();

             services.AddSingleton<DepartmentType>();

             services.AddSingleton<StudentType>();


        }
    }
}