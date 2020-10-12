using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Nekonya.WebApi
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
            services.AddControllers();

            //Oidc
            if (Configuration.GetValue<bool>("Oidc:Enable"))
                services.AddOidcAuthentication(Configuration);

            //Swagger
            if (Configuration.GetValue<bool>("Swagger:Enable"))
                services.AddSwagger(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //Swagger
            if (Configuration.GetValue<bool>("Swagger:Enable"))
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/Swagger/v1/swagger.json", "Swagger: API v1");
                    c.OAuthClientId(Configuration["Swagger:Oidc:ClientId"]);
                    c.OAuthAppName(Configuration["Swagger:Oidc:AppDisplayName"]);
                    //PKCE?
                    if (Configuration.GetValue<bool>("Swagger:Oidc:AuthorizationCode") && Configuration.GetValue<bool>("Swagger:Oidc:PKCE"))
                        c.OAuthUsePkce();
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //Oidc
            if (Configuration.GetValue<bool>("Oidc:Enable"))
            {
                app.UseAuthentication();
            }
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
