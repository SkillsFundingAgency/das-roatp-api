using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories;

[ExcludeFromCodeCoverage]
public sealed class CourseProviderDetailsReadRepository : ICourseProviderDetailsReadRepository
{
    private readonly RoatpDataContext _roatpDataContext;

    private const string GetCourseProviderDetailsStoreProcedureName = "dbo.GetCourseProviderDetails";

    public CourseProviderDetailsReadRepository(RoatpDataContext roatpDataContext)
    {
        _roatpDataContext = roatpDataContext;
    }

    public async Task<List<CourseProviderDetailsModel>> GetCourseProviderDetails(GetCourseProviderDetailsParameters parameters, CancellationToken cancellationToken)
    {
        var connection = _roatpDataContext.Database.GetDbConnection();

        await using DbCommand command = connection.CreateCommand();

        command.CommandText = GetCourseProviderDetailsStoreProcedureName;
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@LarsCode", parameters.LarsCode));
        command.Parameters.Add(new SqlParameter("@Ukprn", parameters.Ukprn));
        command.Parameters.Add(new SqlParameter("@Latitude", parameters.Lat ?? (object)DBNull.Value));
        command.Parameters.Add(new SqlParameter("@Longitude", parameters.Lon ?? (object)DBNull.Value));
        command.Parameters.Add(new SqlParameter("@Location", parameters.Location ?? (object)DBNull.Value));
        command.Parameters.Add(new SqlParameter("@UserId", parameters.ShortlistUserId));

        if (command.Connection!.State != ConnectionState.Open)
        {
            await command.Connection.OpenAsync(cancellationToken);
        }

        var providerDetails = new List<CourseProviderDetailsModel>();

        try
        {
            await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                var model = new CourseProviderDetailsModel
                {
                    Ukprn = reader.GetInt32(nameof(CourseProviderDetailsModel.Ukprn)),
                    ProviderName = reader.GetString(nameof(CourseProviderDetailsModel.ProviderName)),
                    MainAddressLine1 = GetReaderStringValue(nameof(CourseProviderDetailsModel.MainAddressLine1), reader),
                    MainAddressLine2 = GetReaderStringValue(nameof(CourseProviderDetailsModel.MainAddressLine2), reader),
                    MainAddressLine3 = GetReaderStringValue(nameof(CourseProviderDetailsModel.MainAddressLine3), reader),
                    MainAddressLine4 = GetReaderStringValue(nameof(CourseProviderDetailsModel.MainAddressLine4), reader),
                    MainTown = GetReaderStringValue(nameof(CourseProviderDetailsModel.MainTown), reader),
                    MainPostcode = GetReaderStringValue(nameof(CourseProviderDetailsModel.MainPostcode), reader),
                    MarketingInfo = GetReaderStringValue(nameof(CourseProviderDetailsModel.MarketingInfo), reader),
                    Email = GetReaderStringValue(nameof(CourseProviderDetailsModel.Email), reader),
                    PhoneNumber = GetReaderStringValue(nameof(CourseProviderDetailsModel.PhoneNumber), reader),
                    Website = GetReaderStringValue(nameof(CourseProviderDetailsModel.Website), reader),
                    CourseName = GetReaderStringValue(nameof(CourseProviderDetailsModel.CourseName), reader),
                    Level = reader.GetInt32(nameof(CourseProviderDetailsModel.Level)),
                    LarsCode = reader.GetInt32(nameof(CourseProviderDetailsModel.LarsCode)),
                    IFateReferenceNumber = GetReaderStringValue(nameof(CourseProviderDetailsModel.IFateReferenceNumber), reader),
                    Period = GetReaderStringValue(nameof(CourseProviderDetailsModel.Period), reader),
                    Leavers = GetReaderStringValue(nameof(CourseProviderDetailsModel.Leavers), reader),
                    AchievementRate = GetReaderStringValue(nameof(CourseProviderDetailsModel.AchievementRate), reader),
                    NationalLeavers = GetReaderStringValue(nameof(CourseProviderDetailsModel.NationalLeavers), reader),
                    NationalAchievementRate = GetReaderStringValue(nameof(CourseProviderDetailsModel.NationalAchievementRate), reader),

                    ReviewPeriod = GetReaderStringValue(nameof(CourseProviderDetailsModel.ReviewPeriod), reader),
                    EmployerReviews = GetReaderStringValue(nameof(CourseProviderDetailsModel.EmployerReviews), reader),
                    EmployerStars = GetReaderStringValue(nameof(CourseProviderDetailsModel.EmployerStars), reader),
                    EmployerRating = GetReaderStringValue(nameof(CourseProviderDetailsModel.EmployerRating), reader),
                    ApprenticeReviews = GetReaderStringValue(nameof(CourseProviderDetailsModel.ApprenticeReviews), reader),
                    ApprenticeStars = GetReaderStringValue(nameof(CourseProviderDetailsModel.ApprenticeStars), reader),
                    ApprenticeRating = GetReaderStringValue(nameof(CourseProviderDetailsModel.ApprenticeRating), reader),

                    AtEmployer = reader.GetBoolean(nameof(CourseProviderDetailsModel.AtEmployer)),
                    BlockRelease = reader.GetBoolean(nameof(CourseProviderDetailsModel.BlockRelease)),
                    DayRelease = reader.GetBoolean(nameof(CourseProviderDetailsModel.DayRelease)),

                    Ordering = reader.GetInt64(nameof(CourseProviderDetailsModel.Ordering)),
                    LocationType = reader.GetInt32(nameof(CourseProviderDetailsModel.LocationType)),
                    CourseLocation = GetReaderStringValue(nameof(CourseProviderDetailsModel.CourseLocation), reader),
                    AddressLine1 = GetReaderStringValue(nameof(CourseProviderDetailsModel.AddressLine1), reader),
                    AddressLine2 = GetReaderStringValue(nameof(CourseProviderDetailsModel.AddressLine2), reader),
                    Town = GetReaderStringValue(nameof(CourseProviderDetailsModel.Town), reader),
                    County = GetReaderStringValue(nameof(CourseProviderDetailsModel.County), reader),
                    Postcode = GetReaderStringValue(nameof(CourseProviderDetailsModel.Postcode), reader),
                    CourseDistance = reader.GetDouble(nameof(CourseProviderDetailsModel.CourseDistance)),
                    ShortlistId = reader[nameof(CourseProviderDetailsModel.ShortlistId)] == DBNull.Value ? null : reader.GetGuid(nameof(CourseProviderDetailsModel.ShortlistId))
                };

                providerDetails.Add(model);
            }
        }
        finally
        {
            if(command.Connection.State == ConnectionState.Open)
            {
                await command.Connection.CloseAsync();
            }
        }

        return providerDetails;
    }

    private static string GetReaderStringValue(string key, DbDataReader reader)
    {
        return reader[key] == DBNull.Value ? null : (string)reader[key];
    }
}
