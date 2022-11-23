using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.Api.Infrastructure;

public class SwaggerHeaderFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Version",
            In = ParameterLocation.Header,
            AllowEmptyValue = false,
            Example = new OpenApiString("1.0"),
            Required = true
        });

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "Authorization",
            In = ParameterLocation.Header,
            AllowEmptyValue = false,
            Example = new OpenApiString("Bearer <token>"),
            Required = true
        });
    }
}
