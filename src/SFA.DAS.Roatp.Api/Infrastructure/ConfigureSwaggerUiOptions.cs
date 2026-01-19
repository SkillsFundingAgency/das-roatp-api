using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace SFA.DAS.Roatp.Api.AppStart;

[ExcludeFromCodeCoverage]
public sealed class ConfigureSwaggerUiOptions : IConfigureOptions<SwaggerUIOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerUiOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public void Configure(SwaggerUIOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions
                      .OrderBy(d => d.GroupName, StringComparer.OrdinalIgnoreCase)
                      .ThenByDescending(d => d.ApiVersion))
        {
            var docName = $"{description.GroupName}V{description.ApiVersion.MajorVersion}";
            options.SwaggerEndpoint($"/swagger/{docName}/swagger.json", $"{description.GroupName} V{description.ApiVersion.MajorVersion}");
        }
        options.RoutePrefix = string.Empty;
    }
}