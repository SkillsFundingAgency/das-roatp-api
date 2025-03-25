using Microsoft.Data.SqlClient;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.Roatp.Data.Repositories;

public sealed class CourseProviderDetailsReadRepository : ICourseProviderDetailsReadRepository
{
    private readonly RoatpDataContext _roatpDataContext;

    public CourseProviderDetailsReadRepository(RoatpDataContext roatpDataContext)
    {
        _roatpDataContext = roatpDataContext;
    }

    public async Task<List<CourseProviderDetailsModel>> GetCourseProviderDetails(GetCourseProviderDetailsParameters parameters, CancellationToken cancellationToken)
    {
        var connection = _roatpDataContext.Database.GetDbConnection();
        await using DbCommand command = connection.CreateCommand();

        command.CommandText = "dbo.GetCourseProviderDetails";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@LarsCode", parameters.LarsCode));
        command.Parameters.Add(new SqlParameter("@Ukprn", parameters.Ukprn));
        command.Parameters.Add(new SqlParameter("@Latitude", parameters.Lat ?? (object)DBNull.Value));
        command.Parameters.Add(new SqlParameter("@Longitude", parameters.Lon ?? (object)DBNull.Value));
        command.Parameters.Add(new SqlParameter("@Location", parameters.Location ?? (object)DBNull.Value));
        command.Parameters.Add(new SqlParameter("@UserId", parameters.UserId));

        if (command.Connection!.State != ConnectionState.Open)
        {
            await command.Connection.OpenAsync(cancellationToken);
        }

        var providerDetails = new List<CourseProviderDetailsModel>();

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            try
            {
                var model = new CourseProviderDetailsModel
                {
                    Ukprn = reader.GetInt32("Ukprn"),
                    ProviderName = reader.GetString("ProviderName"),
                    MainAddressLine1 = GetReaderStringValue("MainAddressLine1", reader),
                    MainAddressLine2 = GetReaderStringValue("MainAddressLine2", reader),
                    MainAddressLine3 = GetReaderStringValue("MainAddressLine3", reader),
                    MainAddressLine4 = GetReaderStringValue("MainAddressLine4", reader),
                    MainTown = GetReaderStringValue("MainTown", reader),
                    MainPostcode = GetReaderStringValue("MainPostcode", reader),
                    MarketingInfo = GetReaderStringValue("MarketingInfo", reader),
                    Email = GetReaderStringValue("Email", reader),
                    PhoneNumber = GetReaderStringValue("PhoneNumber", reader),
                    Website = GetReaderStringValue("Website", reader),
                    CourseName = GetReaderStringValue("CourseName", reader),
                    Level = reader.GetInt32("Level"),
                    LarsCode = reader.GetInt32("LarsCode"),
                    IFateReferenceNumber = GetReaderStringValue("IFateReferenceNumber", reader),
                    Period = GetReaderStringValue("Period", reader),
                    Leavers = GetReaderStringValue("Leavers", reader),
                    AchievementRate = GetReaderStringValue("AchievementRate", reader),
                    NationalLeavers = GetReaderStringValue("NationalLeavers", reader),
                    NationalAchievementRate = GetReaderStringValue("NationalAchievementRate", reader),

                    ReviewPeriod = GetReaderStringValue("ReviewPeriod", reader),
                    EmployerReviews = GetReaderStringValue("EmployerReviews", reader),
                    EmployerStars =  GetReaderStringValue("EmployerStars", reader),
                    EmployerRating = GetReaderStringValue("EmployerRating", reader),
                    ApprenticeReviews = GetReaderStringValue("ApprenticeReviews", reader),
                    ApprenticeStars = GetReaderStringValue("ApprenticeStars", reader),
                    ApprenticeRating = GetReaderStringValue("ApprenticeRating", reader),

                    AtEmployer = reader.GetBoolean("AtEmployer"),
                    BlockRelease = reader.GetBoolean("BlockRelease"),
                    DayRelease = reader.GetBoolean("DayRelease"),

                    Ordering = reader.GetInt64("Ordering"),
                    LocationType = reader.GetInt32("LocationType"),
                    CourseLocation = GetReaderStringValue("CourseLocation", reader),
                    AddressLine1 = GetReaderStringValue("AddressLine1", reader),
                    AddressLine2 = GetReaderStringValue("AddressLine2", reader),
                    Town = GetReaderStringValue("Town", reader),
                    County = GetReaderStringValue("County", reader),
                    Postcode = GetReaderStringValue("Postcode", reader),
                    CourseDistance = reader.GetDouble("CourseDistance"),
                    ShortlistId = reader["ShortlistId"] == DBNull.Value ? null : reader.GetGuid("ShortlistId")
                };

                providerDetails.Add(model);
            }
            catch (Exception ex)
            {

            }
           
        }

        return providerDetails;
    }

    private static string GetReaderStringValue(string key, DbDataReader reader)
    {
        return reader[key] == DBNull.Value ? null : (string)reader[key];
    }
}
