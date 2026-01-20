using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.Roatp.Api.Infrastructure;

[ExcludeFromCodeCoverage]
public class SwaggerHeaderFilter : IOperationFilter
{
    private const string VersionHeaderName = "X-Version";

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation == null) throw new ArgumentNullException(nameof(operation));

        operation.Parameters ??= new List<OpenApiParameter>();

        var endpointMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;

        var mappedMajors = ApiVersionMetadata.SupportedAPIVersions(endpointMetadata);

        var defaultMajor = mappedMajors.Length > 0 ? mappedMajors.First() : 1;
        var defaultValue = $"{defaultMajor}.0";

        var existing = operation.Parameters
            .FirstOrDefault(p => string.Equals(p.Name, VersionHeaderName, StringComparison.OrdinalIgnoreCase)
                                 && p.In == ParameterLocation.Header);

        if (existing is null)
        {
            operation.Parameters.Insert(0, new OpenApiParameter
            {
                Name = VersionHeaderName,
                In = ParameterLocation.Header,
                Description = $"Example: {defaultValue}",
                Required = false,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Default = new OpenApiString(defaultValue),
                    Example = new OpenApiString(defaultValue)
                }
            });
        }
        else
        {
            existing.Description = $"Example: {defaultValue}";
            existing.Required = false;

            existing.Schema ??= new OpenApiSchema { Type = "string" };
            existing.Schema.Type = "string";
            existing.Schema.Default = new OpenApiString(defaultValue);
            existing.Schema.Example = new OpenApiString(defaultValue);
        }
    }
}