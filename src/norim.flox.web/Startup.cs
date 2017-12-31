using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using norim.flox.core.Configuration;
using norim.flox.web.Infrastructure;
using norim.flox.web.Middlewares;

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

            services.AddMvc(options =>
            {
                options.Conventions.Add(new FeatureConvention());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name:"FeatureDefaultRoute",
                    template: "_{feature}/{controller}/{action=Get}",
                    constraints: null,
                    defaults: null,
                    dataTokens: new { Namespace = "norim.flox.web.Features" });
            });
        }
    }
}
