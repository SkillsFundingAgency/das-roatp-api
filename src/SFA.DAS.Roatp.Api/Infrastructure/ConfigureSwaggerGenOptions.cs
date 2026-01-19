using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.Roatp.Api.Infrastructure;

[ExcludeFromCodeCoverage]
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

        // Include actions based on ApiExplorer.GroupName ("Integration"/"Management")
        // Match docs with suffix "V{major}" by comparing the prefix to apiDesc.GroupName
        options.DocInclusionPredicate((docName, apiDesc) =>
        {
            var dashIndex = docName.IndexOf('V');
            var groupPrefix = dashIndex > 0 ? docName[..dashIndex] : docName;
            return string.Equals(apiDesc.GroupName, groupPrefix, StringComparison.OrdinalIgnoreCase);
        });

        options.OperationFilter<SwaggerHeaderFilter>();
    }
}