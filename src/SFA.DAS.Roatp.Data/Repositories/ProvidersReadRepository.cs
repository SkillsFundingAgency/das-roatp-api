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

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class ProvidersReadRepository : IProvidersReadRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public ProvidersReadRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }
        public async Task<Provider> GetByUkprn(int ukprn)
        {
            return await _roatpDataContext.Providers
                        .Include(p => p.ProviderAddress)
                        .Include(p => p.Courses)
                        .AsNoTracking().SingleOrDefaultAsync(p => p.Ukprn == ukprn);
        }

        public async Task<List<Provider>> GetAllProviders()
        {
            return await _roatpDataContext.Providers
                        .Include(p => p.ProviderAddress)
                        .AsNoTracking().ToListAsync();
        }

        public async Task<List<ProviderSearchModel>> GetProvidersByLarsCode(string larsCode, ProviderOrderBy sortOrder,
                                    GetProvidersFromLarsCodeOptionalParameters parameters, CancellationToken cancellationToken)
        {
            var connection = _roatpDataContext.Database.GetDbConnection();
            await using DbCommand command = connection.CreateCommand();

            command.CommandText = "dbo.GetProvidersByLarsCode";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@larsCode", larsCode));
            command.Parameters.Add(new SqlParameter("@sortOrder", sortOrder.ToString()));

            command.Parameters.Add(new SqlParameter("@Page", parameters.Page));
            command.Parameters.Add(new SqlParameter("@PageSize", parameters.PageSize));
            command.Parameters.Add(new SqlParameter("@workplace", parameters.IsWorkplace));
            command.Parameters.Add(new SqlParameter("@provider", parameters.IsProvider));
            command.Parameters.Add(new SqlParameter("@dayrelease", parameters.IsDayRelease));
            command.Parameters.Add(new SqlParameter("@blockrelease", parameters.IsBlockRelease));
            command.Parameters.Add(new SqlParameter("@Latitude", parameters.Latitude));
            command.Parameters.Add(new SqlParameter("@Longitude", parameters.Longitude));
            command.Parameters.Add(new SqlParameter("@Location", parameters.Location));
            command.Parameters.Add(new SqlParameter("@distance", parameters.Distance));
            command.Parameters.Add(new SqlParameter("@QarRange", parameters.QarRange));
            command.Parameters.Add(new SqlParameter("@employerProviderRatings", parameters.EmployerProviderRatings));
            command.Parameters.Add(new SqlParameter("@apprenticeProviderRatings", parameters.ApprenticeProviderRatings));
            command.Parameters.Add(new SqlParameter("@UserId", parameters.UserId));

            if (command.Connection!.State != ConnectionState.Open)
            {
                await command.Connection.OpenAsync(cancellationToken);
            }

            var pagedProviderDetails = new List<ProviderSearchModel>();
            try
            {
                await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
                while (await reader.ReadAsync(cancellationToken))
                {
                    if (reader["Providers.Ordering"] == DBNull.Value)
                    {
                        pagedProviderDetails.Add(
                            new ProviderSearchModel
                            {
                                Page = (int)reader["Page"],
                                PageSize = (int)reader["PageSize"],
                                TotalPages = (int)reader["TotalPages"],
                                TotalCount = (int)reader["TotalCount"],
                                LarsCode = (string)reader["LarsCode"],
                                StandardName = (string)reader["StandardName"],
                                QarPeriod = (string)reader["QarPeriod"],
                                ReviewPeriod = (string)reader["reviewPeriod"],
                            });
                    }
                    else
                    {
                        pagedProviderDetails.Add(
                            new ProviderSearchModel
                            {
                                Page = (int)reader["Page"],
                                PageSize = (int)reader["PageSize"],
                                TotalPages = (int)reader["TotalPages"],
                                TotalCount = (int)reader["TotalCount"],
                                LarsCode = (string)reader["LarsCode"],
                                StandardName = (string)reader["StandardName"],
                                QarPeriod = (string)reader["QarPeriod"],
                                ReviewPeriod = (string)reader["reviewPeriod"],
                                Ordering = (long)reader["Providers.Ordering"],
                                Ukprn = (int)reader["providers.ukprn"],
                                LocationsCount = (int)reader["providers.locationsCount"],
                                ProviderName = (string)reader["providers.ProviderName"],
                                LocationTypes = (string)reader["providers.locations.locationType"],
                                CourseDistances = (string)reader["providers.locations.courseDistances"],
                                AtEmployers = (string)reader["providers.locations.atEmployer"],
                                DayReleases = (string)reader["providers.locations.dayRelease"],
                                BlockReleases = (string)reader["providers.locations.blockRelease"],
                                Leavers = (string)reader["providers.leavers"],
                                AchievementRate = (string)reader["providers.achievementRate"],
                                EmployerReviews = (string)reader["providers.employerReviews"],
                                EmployerStars = (string)reader["providers.employerStars"],
                                EmployerRating = (ProviderRating)Enum.Parse(typeof(ProviderRating), (string)reader["providers.employerRating"]),
                                ApprenticeReviews = (string)reader["providers.apprenticeReviews"],
                                ApprenticeStars = (string)reader["providers.apprenticeStars"],
                                ApprenticeRating = (ProviderRating)Enum.Parse(typeof(ProviderRating), (string)reader["providers.apprenticeRating"]),
                                ShortlistId = reader["providers.ShortlistId"] != DBNull.Value ? (Guid)reader["providers.shortlistId"] : null
                            });
                    }
                }
            }
            finally
            {
                if (command.Connection.State == ConnectionState.Open)
                {
                    await command.Connection.CloseAsync();
                }
            }
            return pagedProviderDetails;
        }
    }
}
