using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Authorization;
using GraphQL.Language.AST;
using GraphQL.Types;
using GraphQL.Validation;

namespace GraphQLApi.Auth
{
    public class AuthValidationRule : IValidationRule
    {
        private readonly IAuthorizationEvaluator _evaluator;

        public AuthValidationRule(IAuthorizationEvaluator evaluator)
        {
            _evaluator = evaluator;
        }

        public Task<INodeVisitor> ValidateAsync(ValidationContext context)
        {
            var userContext = context.UserContext as IProvideClaimsPrincipal;

            return Task.FromResult((INodeVisitor)new EnterLeaveListener(_ =>
            {
                var operationType = OperationType.Query;

                _.Match<Operation>(astType =>
                {
                    operationType = astType.OperationType;

                    var type = context.TypeInfo.GetLastType();
                    CheckAuth(astType, type, userContext, context, operationType);
                });

                _.Match<ObjectField>(objectFieldAst =>
                {
                    var argumentType = context.TypeInfo.GetArgument().ResolvedType.GetNamedType() as IComplexGraphType;
                    if (argumentType == null)
                        return;

                    var fieldType = argumentType.GetField(objectFieldAst.Name);
                    CheckAuth(objectFieldAst, fieldType, userContext, context, operationType);
                });

                _.Match<Field>(fieldAst =>
                {
                    var fieldDef = context.TypeInfo.GetFieldDef();

                    if (fieldDef == null) return;

                    // check target field
                    CheckAuth(fieldAst, fieldDef, userContext, context, operationType);
                    // check returned graph type
                    CheckAuth(fieldAst, fieldDef.ResolvedType.GetNamedType(), userContext, context, operationType);
                });
            }));
        }

        private void CheckAuth(
            INode node,
            IProvideMetadata type,
            IProvideClaimsPrincipal userContext,
            ValidationContext context,
            OperationType operationType)
        {
            if (type == null || !type.RequiresAuthorization()) return;

            var result = type
                .Authorize(userContext?.User, context.UserContext, null, _evaluator) //context.Inputs
                .GetAwaiter()
                .GetResult();

            if (result.Succeeded) return;

            //var errors = string.Join("\n", result.Errors); // contain revealing information so not using 

            var failedPolicies = new List<string>();
            try
            {
                var authPolicies = type.Metadata.FirstOrDefault(_ => _.Key == "Authorization__Policies");
                failedPolicies = authPolicies.Value as List<string>;
            }
            catch
            {
                failedPolicies?.Add("Unknown");
            }
   
            var failedPolicyReport = string.Join("\n", failedPolicies ?? new List<string>());

            var errorMessage = $"You are not authorized to run this {operationType.ToString().ToLower()}." +
                               $"\nRequired policies: " +
                               $"\n{failedPolicyReport}";

            if (node.GetType() == typeof(Field))
            {
                errorMessage += $"\nField: {((Field)node).Name}";
            }

            context.ReportError(new ValidationError(
                context.OriginalQuery,
                "authorization",
                errorMessage, node));
        }
    }
}

