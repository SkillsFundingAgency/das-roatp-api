using System.Collections.Generic;
using System.Linq;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.Roatp.Api.Infrastructure;

public static class ApiVersionMetadata
{
    public static int[] SupportedAPIVersions(EndpointMetadataCollection metadata)
    {
        var mappedMajors = metadata
            .OfType<MapToApiVersionAttribute>()
            .SelectMany(a => a.Versions)
            .Select(v => v.MajorVersion)
            .Where(m => m.HasValue)
            .Select(m => m!.Value)
            .Distinct()
            .ToArray();

        if (mappedMajors.Length == 0)
        {
            mappedMajors = metadata
                .OfType<ApiVersionAttribute>()
                .SelectMany(a => a.Versions)
                .Select(v => v.MajorVersion)
                .Where(m => m.HasValue)
                .Select(m => m!.Value)
                .Distinct()
                .ToArray();
        }

        return mappedMajors;
    }

    public static int[] SupportedAPIVersions(IList<object> endpointMetadata)
    {
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

        return mappedMajors;
    }
}