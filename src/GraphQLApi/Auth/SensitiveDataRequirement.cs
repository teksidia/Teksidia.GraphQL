using System.Linq;
using System.Threading.Tasks;
using GraphQL.Authorization;

namespace GraphQLApi.Auth
{
    public class SensitiveDataRequirement : IAuthorizationRequirement
    {
        private readonly string[] _whitelistOfClientIds;

        // look for the Application ID of the Azure App Registration registered as a client app
        private const string CLIENT_ID_CLAIM = "http://schemas.microsoft.com/identity/claims/tenantid";

        public SensitiveDataRequirement(string[] whitelistOfClientIds)
        {
            _whitelistOfClientIds = whitelistOfClientIds;
        }

        public Task Authorize(AuthorizationContext context)
        {
            if (_whitelistOfClientIds == null || _whitelistOfClientIds.All(string.IsNullOrEmpty))
            {
                return Task.CompletedTask; // only invoke if whitelist exists!
            }

            foreach (var identity in context.User.Identities)
            {
                foreach (var clientId in _whitelistOfClientIds)
                {
                    if (identity.HasClaim(CLIENT_ID_CLAIM, clientId))
                    {
                        return Task.CompletedTask;
                    }
                }
            }

            context.ReportError("The user does not have permission to view sensitive data.");
            return Task.CompletedTask;
        }
    }
}
