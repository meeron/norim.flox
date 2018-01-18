using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using norim.flox.core.Configuration;
using norim.flox.domain;
using norim.flox.web.Infrastructure;
using norim.flox.web.Middlewares;
using norim.flox.web.Services;

namespace norim.flox.web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ISettings>(x => new Settings(Configuration));
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IFileRepository, domain.Implementations.MongoDbRepository>();

            services.AddMvc(options =>
            {
                options.OutputFormatters.Clear();

                options.Conventions.Add(new FeatureConvention());
                options.OutputFormatters.Add(new JsonCamelCaseOutputFormatter());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.Use(async (context, next) =>
            {
                context.Items["RequestId"] = Guid.NewGuid().ToString();
                context.Response.Headers.Add("RequestId", context.Items["RequestId"].ToString());
                
                await next.Invoke();
            });

            app.UseMiddleware<ErrorHandlerMiddleware>();
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name:"FeatureDefault",
                    template: "_{feature}/{controller}/{action=Get}",
                    constraints: null,
                    defaults: null,
                    dataTokens: new { Namespace = "norim.flox.web.Features" });

                routes.MapRoute(
                    name:"Files",
                    template: "{container}/{*resourceKey}",
                    defaults: new { controller = "Home", action = "GetFile" });

                routes.MapRoute(
                    name:"Default",
                    template: "{controller=Home}/{action=Index}");               
            });
        }
    }
}
