using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nekonya.WebApi.Swagger
{
    public class AuthResponsesOperationFilter : IOperationFilter
    {
        private readonly IConfiguration _Configuration;
        public AuthResponsesOperationFilter(IConfiguration configuration)
        {
            _Configuration = configuration;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAuthorize = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<AuthorizeAttribute>()
                .Any();

            if (hasAuthorize)
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "暂无权限访问" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "禁止访问" });

                List<string> list_scopes = new List<string>();
                var conf_scopes_arr = this._Configuration.GetSection("Swagger:Oidc:Scopes").Get<string[]>();
                if (conf_scopes_arr != null && conf_scopes_arr.Length > 0)
                    list_scopes.AddRange(conf_scopes_arr);

                operation.Security = new List<OpenApiSecurityRequirement>();
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oidc" }
                        },
                        list_scopes.ToArray()
                    }
                });
            }
        }
    }
}
