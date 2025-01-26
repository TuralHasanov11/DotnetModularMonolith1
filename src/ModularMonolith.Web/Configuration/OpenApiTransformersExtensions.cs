using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace ModularMonolith.Web.Configuration;

public static class OpenApiTransformersExtensions
{
    private static OpenApiSecurityScheme JwtBearerScheme => new()
    {
        Type = SecuritySchemeType.Http,
        Name = "Authorization",
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme,
        },
        In = ParameterLocation.Header,
        BearerFormat = "Json Web Token",
        Description = "JWT Authorization: Bearer {token}",
    };

    private static OpenApiSecurityScheme ApiKeyScheme => new()
    {
        Description = "API Key access to API",
        Name = "x-api-key",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKey",
        Reference = new OpenApiReference
        {
            Id = "ApiKey",
            Type = ReferenceType.SecurityScheme,
        },
    };

    public static OpenApiOptions UseBearerSecurityScheme(this OpenApiOptions options)
    {
        var jwtScheme = JwtBearerScheme;

        options.AddDocumentTransformer((doc, _, _) =>
        {
            doc.Components ??= new();
            doc.Components.SecuritySchemes.Add(jwtScheme.Scheme, jwtScheme);

            return Task.CompletedTask;
        });

        options.AddOperationTransformer((operation, context, _) =>
        {
            if (context.Description.ActionDescriptor.EndpointMetadata.OfType<IAuthorizeData>().Any())
            {
                operation.Security = [
                    new OpenApiSecurityRequirement
                    {
                        [jwtScheme] = [],
                    },
                ];
            }

            return Task.CompletedTask;
        });

        return options;
    }

    public static OpenApiOptions UseApiKeySecurityScheme(this OpenApiOptions options)
    {
        var apiKeyScheme = ApiKeyScheme;

        options.AddDocumentTransformer((doc, _, _) =>
        {
            doc.Components ??= new();
            doc.Components.SecuritySchemes.Add(apiKeyScheme.Scheme, apiKeyScheme);

            return Task.CompletedTask;
        });

        return options;
    }

    public static void UseDocumentDetails(this OpenApiOptions options, OpenApiInfo openApiInfoOptions)
    {
        options.AddDocumentTransformer((document, _, _) =>
        {
            document.Info = new()
            {
                Title = openApiInfoOptions.Title,
                Version = openApiInfoOptions.Version,
                Description = openApiInfoOptions.Description,
                Contact = new()
                {
                    Name = openApiInfoOptions.Contact.Name,
                    Email = openApiInfoOptions.Contact.Email,
                    Url = openApiInfoOptions.Contact.Url,
                },
                License = new()
                {
                    Name = openApiInfoOptions.License.Name,
                    Url = openApiInfoOptions.License.Url,
                },
            };
            return Task.CompletedTask;
        });
    }
}
