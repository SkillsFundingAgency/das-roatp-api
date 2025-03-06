﻿using System;
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

namespace SFA.DAS.Roatp.Data.Repositories;

[ExcludeFromCodeCoverage]
public class ShortlistsRepository(RoatpDataContext _roatpDataContext) : IShortlistsRepository
{
    public async Task Create(Shortlist shortlist, CancellationToken cancellationToken)
    {
        _roatpDataContext.Shortlists.Add(shortlist);
        await _roatpDataContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Shortlist> Get(Guid userId, int ukprn, int larsCode, string locationDescription, CancellationToken cancellationToken)
        => _roatpDataContext
            .Shortlists
            .Where(s => s.UserId == userId && s.Ukprn == ukprn && s.LarsCode == larsCode && s.LocationDescription == locationDescription)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<int> GetShortlistCount(Guid userId, CancellationToken cancellationToken)
        => _roatpDataContext.Shortlists.CountAsync(s => s.UserId == userId, cancellationToken);

    public Task Delete(Guid shortlistId, CancellationToken cancellationToken)
        => _roatpDataContext.Shortlists.Where(s => s.Id == shortlistId).ExecuteDeleteAsync(cancellationToken);

    public async Task<string> GetShortlistsForUser(Guid userId, CancellationToken cancellationToken)
    {
        var connection = _roatpDataContext.Database.GetDbConnection();
        await using DbCommand command = connection.CreateCommand();

        command.CommandText = "dbo.GetShortlistsForUser";
        command.CommandType = System.Data.CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@userId", userId));

        if (command.Connection!.State != ConnectionState.Open)
        {
            await command.Connection.OpenAsync(cancellationToken);
        }

        using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        if (reader.Read())
        {
            return reader.GetString(0);
        }
        return string.Empty;
    }
}
