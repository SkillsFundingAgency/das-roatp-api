using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Data.Repositories;

[ExcludeFromCodeCoverage]
internal class ProviderCoursesTimelineRepository(RoatpDataContext _roatpDataContext) : IProviderCoursesTimelineRepository
{
    private const string GetProviderTimelineExportStoredProcedure = "dbo.GetProviderTimelineExport";
    public async Task<List<ProviderTimelineExport>> GetProviderTimelineExport(int? ukprn, CancellationToken cancellationToken)
    {
        var connection = _roatpDataContext.Database.GetDbConnection();

        await using DbCommand command = connection.CreateCommand();

        command.CommandText = GetProviderTimelineExportStoredProcedure;
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@Ukprn", ukprn ?? (object)DBNull.Value));

        if (command.Connection!.State != ConnectionState.Open)
        {
            await command.Connection.OpenAsync(cancellationToken);
        }

        var providerTimelines = new List<ProviderTimelineExport>();

        try
        {
            await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                var model = new ProviderTimelineExport
                {
                    Ukprn = reader.GetInt32(nameof(ProviderTimelineExport.Ukprn)),
                    StatusId = reader.GetInt32(nameof(ProviderTimelineExport.StatusId)),
                    ProviderTypeId = reader.GetInt32(nameof(ProviderTimelineExport.ProviderTypeId)),
                    CourseType = reader[nameof(ProviderTimelineExport.CourseType)] == DBNull.Value
                        ? null
                        : Enum.Parse<CourseType>(reader.GetString(reader.GetOrdinal(nameof(ProviderTimelineExport.CourseType)))),
                    LarsCode = GetReaderStringValue(nameof(ProviderTimelineExport.LarsCode), reader),
                    EffectiveFrom = GetReaderDateTimeValue(nameof(ProviderTimelineExport.EffectiveFrom), reader),
                    EffectiveTo = GetReaderDateTimeValue(nameof(ProviderTimelineExport.EffectiveTo), reader),
                    LastDateStarts = GetReaderDateTimeValue(nameof(ProviderTimelineExport.LastDateStarts), reader)
                };

                providerTimelines.Add(model);
            }
        }
        finally
        {
            if (command.Connection.State == ConnectionState.Open)
            {
                await command.Connection.CloseAsync();
            }
        }

        return providerTimelines;
    }

    private static string GetReaderStringValue(string key, DbDataReader reader)
    {
        return reader[key] == DBNull.Value
            ? null
            : reader.GetString(reader.GetOrdinal(key));
    }

    private static DateTime? GetReaderDateTimeValue(string key, DbDataReader reader)
    {
        return reader[key] == DBNull.Value
            ? null
            : reader.GetDateTime(reader.GetOrdinal(key));
    }
}
