using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.ComponentModel.DataAnnotations;
using DataModel;

namespace CodeGeneratorUtility
{
    class Program
    {
        public static string EntityTypeTemplate { get; set; }
        public static string EntityTypeChildTemplate { get; set; }
        public static string DataQueryTemplate { get; set; }
        public static string StartupServicesTemplate { get; set; }

        public static string TargetLocation { get; set; }

        static void Main(string[] args)
        {
            EntityTypeTemplate = File.ReadAllText(Path.Join(Environment.CurrentDirectory, "EntityTypeTemplate.txt"));
            EntityTypeChildTemplate = File.ReadAllText(Path.Join(Environment.CurrentDirectory, "EntityTypeChildTemplate.txt"));
            DataQueryTemplate = File.ReadAllText(Path.Join(Environment.CurrentDirectory, "DataQueryTemplate.txt"));
            StartupServicesTemplate = File.ReadAllText(Path.Join(Environment.CurrentDirectory, "StartupServicesTemplate.txt"));

            TargetLocation = GetGeneratedCodeTargetLocation();

            var models = Assembly.GetAssembly(typeof(Student));
            var modelTypes = models.GetTypes();

            var queries = new StringBuilder();
            var initServices = new StringBuilder();

            foreach (var modelType in modelTypes)
            {
                if (Attribute.IsDefined(modelType, typeof(EntityAttribute)))
                {
                   CreateGraphQLType(modelType);

                   var query = "            InitQuery<{0}Type, {0}>();\n";
                   var completedQuery = string.Format(query, modelType.Name);
                   queries.AppendLine(completedQuery);

                   var init = "             services.AddSingleton<{0}Type>();\n";
                   var completedInit = string.Format(init, modelType.Name);
                   initServices.AppendLine(completedInit);
                }
            }

            // create root query

            var rootQueryFile = "DataQuery.cs";
            var queryCode = string.Format(DataQueryTemplate, queries);
            File.WriteAllText($"{TargetLocation}\\GraphQL\\{rootQueryFile}", queryCode);
            Console.WriteLine($"Created root query object {rootQueryFile}");

            // create dependency injection config

            var startUpFile = "StartUp.Services.cs";
            var initCode = string.Format(StartupServicesTemplate, initServices);
            File.WriteAllText($"{TargetLocation}\\{startUpFile}", initCode);
            Console.WriteLine($"Created DI config in {startUpFile}");

            Console.WriteLine("Complete...");
            Console.ReadLine();
        }

        private static string GetGeneratedCodeTargetLocation()
        {
            var currentDir = Environment.CurrentDirectory;
            var projectName = Assembly.GetCallingAssembly().GetName().Name;
            var path = currentDir.Substring(0, currentDir.IndexOf(projectName, StringComparison.Ordinal));
            return Path.Join(path, "GraphQLApi");
        }

        private static void CreateGraphQLType(Type modelType)
        {
            var propertyInfos = modelType.GetProperties();
            var fields = new StringBuilder();

            foreach (var prop in propertyInfos)
            {
                var field = "            Field(h => h.{0}{1}).Description(\"{0}\");\r\n";
                var nullsAllowed =
                    AllowNullables(prop.PropertyType, prop.GetCustomAttributes<KeyAttribute>().Any());
                var completedField = string.Format(field, prop.Name, nullsAllowed ? ", true" : "");
                fields.AppendLine(completedField);
            }

            var mainClassGeneratedCode = string.Format(EntityTypeTemplate, modelType.Name, fields);

            // create main query fields

            var fileName = $"{modelType.Name}Type.cs";
            File.WriteAllText($"{TargetLocation}\\Types\\{fileName}", mainClassGeneratedCode);
            Console.WriteLine($"Created GraphQL type {fileName}");

            // create partial classes for custom fields (e.g. children of main entity)

            var childFieldsFileName = $"{modelType.Name}Type.Children.cs";
            if (!File.Exists($"{TargetLocation}\\Types\\{childFieldsFileName}"))
            {
                var childClassGeneratedCode = string.Format(EntityTypeChildTemplate, modelType.Name);
                File.WriteAllText($"{TargetLocation}\\Types\\{childFieldsFileName}", childClassGeneratedCode);
                Console.WriteLine($"Created GraphQL type partial for custom fields {fileName}");
            }

        }

        internal static bool AllowNullables(Type type, bool isPrimaryKey)
        {
            var isNullableType = !type.IsValueType || IsNullableType(type);
            return !isPrimaryKey && isNullableType;
        }

        internal static bool IsNullableType(Type type)
        {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
