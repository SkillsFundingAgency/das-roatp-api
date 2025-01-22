using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories;

[ExcludeFromCodeCoverage]
public sealed class ProvidersCountReadRepository : IProvidersCountReadRepository
{
    private readonly RoatpDataContext _roatpDataContext;

    public ProvidersCountReadRepository(RoatpDataContext roatpDataContext)
    {
        _roatpDataContext = roatpDataContext;
    }

    public async Task<List<CourseInformation>> GetProviderTrainingCourses(int[] larsCodes, decimal? longitude, decimal? latitude, int? distance, CancellationToken cancellationToken)
    {
        var connection = _roatpDataContext.Database.GetDbConnection();
        await using var command = connection.CreateCommand();

        command.CommandText = "dbo.GetTrainingProvidersCount";
        command.CommandType = System.Data.CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@Latitude", latitude ?? (object)DBNull.Value));
        command.Parameters.Add(new SqlParameter("@Longitude", longitude ?? (object)DBNull.Value));
        command.Parameters.Add(new SqlParameter("@Distance", distance ?? (object)DBNull.Value));
        command.Parameters.Add(new SqlParameter("@LarsCodes", string.Join(',', larsCodes)));

        if (command.Connection.State != System.Data.ConnectionState.Open)
        {
            await command.Connection.OpenAsync(cancellationToken);
        }

        var courseInformation = new List<CourseInformation>();
        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            courseInformation.Add(
                new CourseInformation(
                    reader.GetInt32(0),
                    reader.GetInt32(1),
                    reader.GetInt32(2)
                )
            );
        }

        return courseInformation;
    }
}
