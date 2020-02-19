using System;
using System.Collections.Generic;
using GraphQL.Validation;
using Microsoft.AspNetCore.Http;

namespace GraphQLApi.GraphQL
{
    public class GraphQLSettings
    {
        public PathString Path { get; set; } = "/graphql";
        public Func<HttpContext, GraphQLUserContext> BuildUserContext { get; set; }
        public bool ExposeExceptions { get; set; }
        public bool EnableMetrics { get; set; }
        public List<IValidationRule> ValidationRules { get; } = new List<IValidationRule>();

        /// <summary>
        /// Used to prevent DoS attacks through overly complex queries that break the server
        /// </summary>
        public int MaxQueryDepth { get; set; }
    }
}
