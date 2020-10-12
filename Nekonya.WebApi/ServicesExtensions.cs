using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Nekonya.WebApi.Swagger;

namespace Nekonya.WebApi
{
    public static class ServicesExtensions
    {
        public static void AddOidcAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = configuration["Oidc:Authority"];
                    options.ApiName = configuration["Oidc:ApiName"];
                    options.RequireHttpsMetadata = configuration.GetValue<bool>("Oidc:RequireHttps");
                });
        }

        public static void AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = configuration["Swagger:v1:Title"],
                    Version = configuration["Swagger:v1:VersionName"],
                    Description = configuration["Swagger:v1:Desc"],
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                if (configuration.GetValue<bool>("Oidc:Enable"))
                {
                    //Auth
                    var dict_scopes = new Dictionary<string, string>();
                    string[] scopes_arr = configuration.GetSection("Swagger:Oidc:Scopes").Get<string[]>();
                    string[] scopes_desc_arr = configuration.GetSection("Swagger:Oidc:ScopesDesc").Get<string[]>();
                    for (var i = 0; i < scopes_arr.Length; i++)
                    {
                        dict_scopes.Add(scopes_arr[i], (scopes_desc_arr.Length > i) ? scopes_desc_arr[i] : scopes_arr[i]);
                    }
                    if (configuration.GetValue<bool>("Swagger:Oidc:AuthorizationCode"))
                    {
                        //Authorization Code Flow | 授权码模式
                        options.AddSecurityDefinition("oidc", new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.OAuth2,
                            Flows = new OpenApiOAuthFlows
                            {
                                AuthorizationCode = new OpenApiOAuthFlow
                                {
                                    AuthorizationUrl = new Uri($"{configuration["Oidc:Authority"]}/connect/authorize"),
                                    TokenUrl = new Uri($"{configuration["Oidc:Authority"]}/connect/token"),
                                    Scopes = dict_scopes
                                }
                            }
                        });
                    }
                    else
                    {
                        //Implicit Flow
                        options.AddSecurityDefinition("oidc", new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.OAuth2,
                            Flows = new OpenApiOAuthFlows
                            {
                                Implicit = new OpenApiOAuthFlow
                                {
                                    AuthorizationUrl = new Uri($"{configuration["Oidc:Authority"]}/connect/authorize"),
                                    Scopes = dict_scopes
                                }
                            }
                        });
                    }
                    

                    options.OperationFilter<AuthResponsesOperationFilter>();
                }
            });
        }

    }
}
