namespace Allycs.Common.Devices.Modules
{
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Nancy;
    using Nancy.Bootstrapper;
    using Nancy.Bootstrappers.Autofac;
    using Nancy.Configuration;
    using Nancy.Conventions;
    using System;

    public partial class Bootstrapper : AutofacNancyBootstrapper
    {
        public static ILifetimeScope BootstrapperLifetimeScope;

        protected readonly IConfiguration Configuration;
        private readonly IServiceCollection _services;

        public Bootstrapper(IServiceCollection services, IConfiguration configuration)
        {
            Configuration = configuration;
            _services = services;
        }

        public override void Configure(INancyEnvironment environment)
        {
            var hostEnv = _services.BuildServiceProvider().GetService<IWebHostEnvironment>();
            if (!hostEnv.IsProduction())
            {
                environment.Tracing(false, true);
            }
        }

        protected override void ConfigureApplicationContainer(ILifetimeScope existingContainer)
        {
            existingContainer.Update(builder =>
            {
                builder.Populate(_services);
                //builder.Register<IDbConnection>((c, p) => new NpgsqlConnection(Configuration["ConnectionString"]));
            });
            base.ConfigureApplicationContainer(existingContainer);
            //var _settings = existingContainer.Resolve<IOptionsSnapshot<AppSettings>>().Value;
            var _logger = existingContainer.Resolve<ILogger<Bootstrapper>>();
            BootstrapperLifetimeScope = existingContainer;
        }

        protected override void ApplicationStartup(ILifetimeScope container, IPipelines pipelines)
        {
            pipelines.AfterRequest.AddItemToEndOfPipeline((ctx) =>
            {
                ctx.Response.WithHeader("Access-Control-Allow-Origin", "*")
                    .WithHeader("Access-Control-Allow-Methods", "*")
                    .WithHeader("Access-Control-Allow-Headers", "*");
            });
            base.ApplicationStartup(container, pipelines);
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);
            nancyConventions.StaticContentsConventions.Clear();
            nancyConventions.StaticContentsConventions.Add
                (StaticContentConventionBuilder.AddDirectory("assets", "/assets"));
        }
    }
}
