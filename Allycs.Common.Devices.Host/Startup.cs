using Allycs.Common.Devices.Modules;
using Allycs.Common.Devices.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Nancy.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace Allycs.Common.Devices.Host
{

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        public IConfiguration Configuration { get; }
        private IServiceCollection _services { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddTimedJob();
            services.Configure<AppSettings>(Configuration);
            services.AddSingleton<PostgresService>();
            services.AddOptions();
          
            var namespacePath = Configuration.GetValue<string>("NamespacePath");
            var jsonServices = JObject.Parse(File.ReadAllText($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json"))["DIServices"];
            var requiredServices = JsonConvert.DeserializeObject<List<IocService>>(jsonServices.ToString());

            foreach (var service in requiredServices)
            {
                services.Add(new ServiceDescriptor(serviceType: Type.GetType(namespacePath + "." + service.ServiceType + "," + namespacePath),
                                  implementationType: Type.GetType(namespacePath + "." + service.ImplementationType + "," + namespacePath),
                                  lifetime: service.Lifetime));
            }

            // If using Kestrel:
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            // If using IIS:
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            _services = services;
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            var appLifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();
            if (appLifetime != null)
            {
                appLifetime.ApplicationStopped.Register(Serilog.Log.CloseAndFlush);
            }
            app.UseOwin().UseNancy(x => x.Bootstrapper = new Bootstrapper(_services, Configuration));
            app.UseTimedJob();
        }
    }
}
