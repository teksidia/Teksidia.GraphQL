using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Instrumentation;
using GraphQL.Types;
using GraphQL.Validation.Complexity;
using Microsoft.AspNetCore.Http;

namespace GraphQLApi.GraphQL
{
    public class GraphQLMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly GraphQLSettings _settings;
        private readonly IDocumentExecuter _executer;
        private readonly IDocumentWriter _writer;
        private readonly DataLoaderDocumentListener _dataLoaderDocumentListener;

        public GraphQLMiddleware(
            RequestDelegate next,
            GraphQLSettings settings,
            IDocumentExecuter executer,
            IDocumentWriter writer,
            DataLoaderDocumentListener dataLoaderDocumentListener)
        {
            _next = next;
            _settings = settings;
            _executer = executer;
            _writer = writer;
            _dataLoaderDocumentListener = dataLoaderDocumentListener;
        }

        public async Task Invoke(HttpContext context, ISchema schema)
        {
            if (!IsGraphQLRequest(context))
            {
                await _next(context);
                return;
            }

            await ExecuteAsync(context, schema);
        }

        private bool IsGraphQLRequest(HttpContext context)
        {
            return context.Request.Path.StartsWithSegments(_settings.Path)
                && string.Equals(context.Request.Method, "POST", StringComparison.OrdinalIgnoreCase);
        }

        private async Task ExecuteAsync(HttpContext context, ISchema schema)
        {
            var start = DateTime.UtcNow;
            var request = await JsonSerializer.DeserializeAsync<GraphQLRequest>(context.Request.Body,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            var userContext = _settings.BuildUserContext.Invoke(context);
            var result = await _executer.ExecuteAsync(_ =>
            {
                _.Schema = schema;
                _.Query = request?.Query;
                _.OperationName = request?.OperationName;
                _.Inputs = request?.Variables.ToInputs();
                _.UserContext = userContext;
                _.ValidationRules = _settings.ValidationRules;
                _.EnableMetrics = _settings.EnableMetrics;
                _.ExposeExceptions = _settings.ExposeExceptions;
                _.ComplexityConfiguration = new ComplexityConfiguration {MaxDepth = _settings.MaxQueryDepth};
                if (_settings.EnableMetrics)
                {
                    _.FieldMiddleware.Use<InstrumentFieldsMiddleware>();
                }

                _.Listeners.Add(_dataLoaderDocumentListener);
            });

            if (_settings.EnableMetrics)
            {
                result.EnrichWithApolloTracing(start);
            }

            await WriteResponseAsync(context, result);
        }

        private async Task WriteResponseAsync(HttpContext context, ExecutionResult result)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = result.Errors?.Any() == true ? (int)HttpStatusCode.BadRequest : (int)HttpStatusCode.OK;

            await _writer.WriteAsync(context.Response.Body, result);
        }
    }
}
