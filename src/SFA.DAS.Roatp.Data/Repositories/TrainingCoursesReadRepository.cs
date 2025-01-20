using Microsoft.Data.SqlClient;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories;

[ExcludeFromCodeCoverage]
public sealed class TrainingCoursesReadRepository : ITrainingCoursesReadRepository
{
    private readonly DbConnection _dbConnection;

    public TrainingCoursesReadRepository(DbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<List<CourseInformation>> GetProviderTrainingCourses(int[] larsCodes, decimal? longitude, decimal? latitude, int? distance, CancellationToken cancellationToken)
    {
        using var command = _dbConnection.CreateCommand();
        command.CommandText = "dbo.GetCoursesByDistance";
        command.CommandType = System.Data.CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@Latitude", latitude));
        command.Parameters.Add(new SqlParameter("@Longitude", longitude));
        command.Parameters.Add(new SqlParameter("@Distance", distance));
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
