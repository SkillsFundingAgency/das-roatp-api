using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Roatp.Api.Infrastructure;

[ExcludeFromCodeCoverage]
public sealed class ApiVersionHeaderValidationMiddleware
{
    private const string HeaderName = "X-Version";
    private readonly RequestDelegate _next;

    public ApiVersionHeaderValidationMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path;

        if (path.StartsWithSegments("/swagger") && path.Value!.EndsWith("/swagger.json"))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(HeaderName, out var values) || values.Count == 0)
        {
            await WriteProblem(context, StatusCodes.Status406NotAcceptable,
                "Invalid or missing API version",
                $"The {HeaderName} header is required. Specify the API version using the '{HeaderName}' header.");
            return;
        }

        if (!double.TryParse(values[0], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var requestedMajor))
        {
            await WriteProblem(context, StatusCodes.Status406NotAcceptable,
                "Invalid or missing API version",
                $"The {HeaderName} header value is not a valid API version. Use version, e.g. '1' or '1.0'.");
            return;
        }

        var endpoint = context.GetEndpoint();
        if (endpoint is null)
        {
            await _next(context);
            return;
        }

        var mappedMajors = ApiVersionMetadata.SupportedAPIVersions(endpoint.Metadata);

        if (mappedMajors.Length == 0)
        {
            await WriteProblem(context, StatusCodes.Status406NotAcceptable,
                "Unsupported API version",
                $"The requested API version '{requestedMajor.ToString("0.0", CultureInfo.InvariantCulture)}' is not supported.");
            return;
        }

        await _next(context);
    }

    private static async Task WriteProblem(HttpContext context, int status, string title, string detail)
    {
        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
        };

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        var json = JsonSerializer.Serialize(problem);
        await context.Response.WriteAsync(json);
    }
}

public static class ApiVersionHeaderValidationMiddlewareExtensions
{
    public static IApplicationBuilder UseApiVersionHeaderValidation(this IApplicationBuilder app) =>
        app.UseMiddleware<ApiVersionHeaderValidationMiddleware>();
}