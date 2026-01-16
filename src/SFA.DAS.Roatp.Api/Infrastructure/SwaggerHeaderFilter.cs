using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

        // Do not add if a header parameter with the same name already exists.
        var alreadyHas = operation.Parameters
            .Any(p => string.Equals(p.Name, VersionHeaderName, StringComparison.OrdinalIgnoreCase)
                      && p.In == ParameterLocation.Header);

        if (alreadyHas) return;

        operation.Parameters.Insert(0, new OpenApiParameter
        {
            Name = VersionHeaderName,
            In = ParameterLocation.Header,
            Description = "API version (header)",
            Required = false,
            Schema = new OpenApiSchema { Type = "string" }
        });
    }
}
