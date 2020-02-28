using System;
using System.Collections.Generic;
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
        public static string DataQueryTemplate { get; set; }
        public static string StartupServicesTemplate { get; set; }
        public static string OneToOneMappingTemplate { get; set; }
        public static string OneToManyMappingTemplate { get; set; }

        public static string TargetLocation { get; set; }

        static void Main(string[] args)
        {
            EntityTypeTemplate = File.ReadAllText(Path.Join(Environment.CurrentDirectory, "EntityTypeTemplate.txt"));
            DataQueryTemplate = File.ReadAllText(Path.Join(Environment.CurrentDirectory, "DataQueryTemplate.txt"));
            StartupServicesTemplate = File.ReadAllText(Path.Join(Environment.CurrentDirectory, "StartupServicesTemplate.txt"));
            OneToOneMappingTemplate = File.ReadAllText(Path.Join(Environment.CurrentDirectory, "OneToOneMappingTemplate.txt"));
            OneToManyMappingTemplate = File.ReadAllText(Path.Join(Environment.CurrentDirectory, "OneToManyMappingTemplate.txt"));

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

                   var tableName = modelType.Name;
                   var query = "            InitQuery<{0}Type, {0}>(\"{1}\");\n";
                   var completedQuery = string.Format(query, modelType.Name, tableName);
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
                var isChildField = Attribute.IsDefined(prop, typeof(ChildAttribute));
                var isEnumerable = typeof(IEnumerable<object>).IsAssignableFrom(prop.PropertyType);

                if (isChildField && isEnumerable)
                {
                    var field = CreateChildField(OneToManyMappingTemplate, modelType.Name, prop, isEnumerable);
                    fields.AppendLine(field);
                }

                if (isChildField && !isEnumerable)
                {
                    var field = CreateChildField(OneToOneMappingTemplate, modelType.Name, prop, isEnumerable);
                    fields.AppendLine(field);
                }

                if (!isChildField)
                {
                    var field = CreateScalarField(prop);
                    fields.AppendLine(field);
                }
            }

            var mainClassGeneratedCode = string.Format(EntityTypeTemplate, modelType.Name, fields);
            var fileName = $"{modelType.Name}Type.cs";
            File.WriteAllText($"{TargetLocation}\\Types\\{fileName}", mainClassGeneratedCode);
            Console.WriteLine($"Created GraphQL type {fileName}");
        }

        private static string CreateScalarField(PropertyInfo prop)
        {
            const string field = "            Field(h => h.{0}{1}).Description(\"{0}\");\r\n";
            var nullsAllowed =
                AllowNullables(prop.PropertyType, prop.GetCustomAttributes<KeyAttribute>().Any());
            return string.Format(field, prop.Name, nullsAllowed ? ", true" : "");
        }

        private static string CreateChildField(string template, string parentEntity, PropertyInfo prop, bool isEnumerable)
        {
            var propertyType = isEnumerable ? prop.PropertyType.GetGenericArguments()[0] : prop.PropertyType;

            var childEntity = propertyType.Name;
            var relationship = prop.GetCustomAttribute<ChildAttribute>();

            var joinCollection = new List<Tuple<string, string>>
            {
                new Tuple<string, string>(relationship.LocalProperty, relationship.ForeignProperty),
                new Tuple<string, string>(relationship.LocalProperty2, relationship.ForeignProperty2),
                new Tuple<string, string>(relationship.LocalProperty3, relationship.ForeignProperty3),
                new Tuple<string, string>(relationship.LocalProperty4, relationship.ForeignProperty4),
                new Tuple<string, string>(relationship.LocalProperty5, relationship.ForeignProperty5)
            };

            var parentJoinCollection = joinCollection.Where(_ => !string.IsNullOrEmpty(_.Item1)).Select(_ => string.Format("{{\"{0}\", context.Source.{1}}}", _.Item2, _.Item1));
            var childJoinCollection = joinCollection.Where(_ => !string.IsNullOrEmpty(_.Item2)).Select(_ => string.Format("{{\"{0}\", r.{0}}}", _.Item2));

            var parentJoins = string.Join(", ", parentJoinCollection);
            var childJoins = string.Join(", ", childJoinCollection);

            var field = string.Format(template, propertyType.Name, prop.Name.ToLowerInvariant(), childEntity, parentEntity, parentJoins, childJoins);
            return field;
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
