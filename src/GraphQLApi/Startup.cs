using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Server.Ui.Playground;
using GraphQL.SystemTextJson;
using GraphQL.Types;
using GraphQL.Validation;
using GraphQLApi.Auth;
using GraphQLApi.GraphQL;
using GraphQLApi.Types.Custom;
using GraphQLApi.Types.Custom.Inputs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GraphQLApi
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddJwtAuthentication(Configuration.GetSection("JwtAuthSettings"));

            services.AddSingleton<IDataLoaderContextAccessor, DataLoaderContextAccessor>();
            services.AddSingleton<DataLoaderDocumentListener>();

            // graphql writers

            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IDocumentWriter, DocumentWriter>();

            // graph types

            // ... custom types
            services.AddSingleton<FilterGraphType>();
            services.AddSingleton(typeof(ResultType<,>), typeof(ResultType<,>));
            services.AddSingleton<PageInfoType>();

            // ... boilerplate types
            InitGraphTypes(services);

            // query factory

            services.AddSingleton(
                new QueryFactoryHelper(Configuration.GetConnectionString("Database")));

            // graph QL components

            services.AddSingleton<ISchema, DataSchema>();
            services.AddSingleton<DataQuery>();

            // graph QL auth config
            services.AddGraphQLAuthPolicies(Configuration["JwtAuthSettings:SensitiveDataClientWhiteList"]);

            services.AddLogging(builder => builder.AddConsole());
            services.AddHttpContextAccessor();
            services.AddHttpClient();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            bool.TryParse(Configuration["GraphQL:ExposeExceptions"], out var exposeExceptions);
            bool.TryParse(Configuration["GraphQL:EnableMetrics"], out var enableMetrics);
            int.TryParse(Configuration["GraphQL:MaxQueryDepth"], out var maxQueryDepth);

            var settings = new GraphQLSettings
            {
                Path = "/graphql",
                BuildUserContext = CreateUserContext,
                EnableMetrics = exposeExceptions,
                ExposeExceptions = enableMetrics,
                MaxQueryDepth = maxQueryDepth == 0 ? 15 : maxQueryDepth
            };
            var validationRules = app.ApplicationServices.GetServices<IValidationRule>();
            settings.ValidationRules.AddRange(validationRules);



            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            //app.UseAuthentication();
            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseMiddleware<GraphQLMiddleware>(settings);

            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions() { Path = new PathString("/ui/playground") });
        }

        private static GraphQLUserContext CreateUserContext(HttpContext httpContext)
        {
            var userContext = new GraphQLUserContext
            {
                User = httpContext.User
            };
            userContext.Add("Example", "Can be accessed & used in AuthValidationRule");
            return userContext;
        }
    }
}
