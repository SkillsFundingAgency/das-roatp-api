using Microsoft.AspNetCore.Builder;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SFA.DAS.Roatp.Api.StartupExtensions
{
    [ExcludeFromCodeCoverage]
    public static class CorrelationIdInMiddleware
    {
        public const string CorrelationIdHeaderKey = "Correlation-Id";
        public static IApplicationBuilder UseCorrelationIdInMiddleware(this IApplicationBuilder builder)
        {
            return builder.Use(async (context, func) =>
            {
                string? correlationId = context.Request.Headers[CorrelationIdHeaderKey].FirstOrDefault();
                if (string.IsNullOrEmpty(correlationId))
                {
                    correlationId = Guid.NewGuid().ToString();
                    context.Request.Headers.Add(CorrelationIdHeaderKey, correlationId);
                }
                context.Response.Headers.Add(CorrelationIdHeaderKey, correlationId);
                await func();
            });
        }
    }
}
