using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
    public async Task<List<ProviderRegistrationDetail>> GetAllProviderCoursesTimelines(CancellationToken cancellationToken)
    {
        return await _roatpDataContext
            .ProviderRegistrationDetails
            .Include(p => p.ProviderCourseTypes)
            .Include(p => p.Provider)
            .ThenInclude(p => p.ProviderCoursesTimelines)
            .ThenInclude(t => t.Standard)
            .Where(r => r.StatusId == (int)ProviderStatusType.Active || r.StatusId == (int)ProviderStatusType.ActiveNoStarts)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProviderRegistrationDetail> GetProviderCoursesTimelines(int ukprn, CancellationToken cancellationToken)
    {
        var result = await _roatpDataContext
            .ProviderRegistrationDetails
            .Include(p => p.ProviderCourseTypes)
            .Include(t => t.Provider)
            .ThenInclude(p => p.ProviderCoursesTimelines)
            .ThenInclude(t => t.Standard)
            .Where(r => (r.StatusId == (int)ProviderStatusType.Active || r.StatusId == (int)ProviderStatusType.ActiveNoStarts) && r.Ukprn == ukprn)
            .ToListAsync(cancellationToken);

        return result.FirstOrDefault();
    }

    public async Task<List<ProviderCoursesTimelineExport>> GetProviderCoursesTimeline(int? ukprn, CancellationToken cancellationToken)
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

        var providerTimelines = new List<ProviderCoursesTimelineExport>();

        try
        {
            await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                var model = new ProviderCoursesTimelineExport
                {
                    Ukprn = reader.GetInt32(nameof(ProviderCoursesTimelineExport.Ukprn)),
                    StatusId = reader.GetInt32(nameof(ProviderCoursesTimelineExport.StatusId)),
                    ProviderTypeId = reader.GetInt32(nameof(ProviderCoursesTimelineExport.ProviderTypeId)),
                    CourseType = reader[nameof(ProviderCoursesTimelineExport.CourseType)] == DBNull.Value
                        ? null
                        : Enum.Parse<CourseType>(reader.GetString(reader.GetOrdinal(nameof(ProviderCoursesTimelineExport.CourseType)))),
                    LarsCode = GetReaderStringValue(nameof(ProviderCoursesTimelineExport.LarsCode), reader),
                    EffectiveFrom = GetReaderDateTimeValue(nameof(ProviderCoursesTimelineExport.EffectiveFrom), reader),
                    EffectiveTo = GetReaderDateTimeValue(nameof(ProviderCoursesTimelineExport.EffectiveTo), reader),
                    LastDateStarts = GetReaderDateTimeValue(nameof(ProviderCoursesTimelineExport.LastDateStarts), reader)
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
