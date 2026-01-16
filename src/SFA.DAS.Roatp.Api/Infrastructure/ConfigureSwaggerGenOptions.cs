using System;
using System.Linq;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.Roatp.Api.Infrastructure;

public class ConfigureSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public void Configure(SwaggerGenOptions options)
    {
        options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

        foreach (var apiVersionDescription in _provider.ApiVersionDescriptions)
        {
            var docName = $"{apiVersionDescription.GroupName}V{apiVersionDescription.ApiVersion.MajorVersion}";

            options.SwaggerDoc(docName, new OpenApiInfo
            {
                Title = $"{apiVersionDescription.GroupName} V{apiVersionDescription.ApiVersion.MajorVersion}",
                Version = apiVersionDescription.ApiVersion.ToString()
            });
        }

        options.OperationFilter<SwaggerHeaderFilter>();
    }
}