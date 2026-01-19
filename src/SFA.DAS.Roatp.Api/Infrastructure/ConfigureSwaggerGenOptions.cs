using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Asp.Versioning;
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
            var vIndex = docName.LastIndexOf('V');
            if (vIndex <= 0) return false;

            var group = docName[..vIndex];
            var versionPart = docName[(vIndex + 1)..];
            if (!int.TryParse(versionPart, out var major)) return false;

            // Must match the controller group
            if (!string.Equals(apiDesc.GroupName, group, StringComparison.OrdinalIgnoreCase))
                return false;

            // Determine supported majors from endpoint metadata
            // Prefer MapToApiVersion at action level; fallback to ApiVersion at controller level
            var endpointMetadata = apiDesc.ActionDescriptor.EndpointMetadata;

            var mappedMajors = endpointMetadata
                .OfType<MapToApiVersionAttribute>()
                .SelectMany(a => a.Versions)
                .Select(v => v.MajorVersion)
                .Where(m => m.HasValue)
                .Select(m => m!.Value)
                .Distinct()
                .ToArray();

            if (mappedMajors.Length == 0)
            {
                mappedMajors = endpointMetadata
                    .OfType<ApiVersionAttribute>()
                    .SelectMany(a => a.Versions)
                    .Select(v => v.MajorVersion)
                    .Where(m => m.HasValue)
                    .Select(m => m!.Value)
                    .Distinct()
                    .ToArray();
            }

            // If no version info is found, exclude to avoid cross-version leakage
            if (mappedMajors.Length == 0) return false;

            return mappedMajors.Contains(major);
        });

        options.OperationFilter<SwaggerHeaderFilter>();
    }
}