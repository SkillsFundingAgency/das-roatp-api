using System;
using System.Collections.Generic;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetCourseProviderDetails;

public sealed class GetCourseProviderDetailsQueryResult
{
    public int Ukprn { get; set; }
    public string ProviderName { get; set; }
    public ShortProviderAddressModel Address { get; set; }
    public ContactModel Contact { get; set; }
    public string CourseName { get; set; }
    public int Level { get; set; }
    public int LarsCode { get; set; }
    public string IFateReferenceNumber { get; set; }
    public QarModel QAR { get; set; }
    public ReviewModel Reviews { get; set; }
    public IEnumerable<LocationModel> Locations { get; set; } = [];
    public Guid? ShortlistId { get; set; }

    public static implicit operator GetCourseProviderDetailsQueryResult(CourseProviderDetailsModel source)
    {
        return new GetCourseProviderDetailsQueryResult
        {
            Ukprn = source.Ukprn,
            ProviderName = source.ProviderName,
            Address = new ShortProviderAddressModel()
            {
                AddressLine1 = source.MainAddressLine1,
                AddressLine2 = source.MainAddressLine2,
                AddressLine3 = source.MainAddressLine3,
                AddressLine4 = source.MainAddressLine4,
                Town = source.MainTown,
                Postcode = source.MainPostcode
            },
            Contact = new ContactModel()
            {
                MarketingInfo = source.MarketingInfo,
                Website = source.Website,
                Email = source.Email,
                PhoneNumber = source.PhoneNumber
            },
            CourseName = source.CourseName,
            Level = source.Level,
            LarsCode = int.TryParse(source.LarsCode, out var l) ? l : 0,
            IFateReferenceNumber = source.IFateReferenceNumber,
            QAR = new QarModel()
            {
                Period = source.Period,
                Leavers = source.Leavers,
                AchievementRate = source.AchievementRate,
                NationalLeavers = source.NationalLeavers,
                NationalAchievementRate = source.NationalAchievementRate
            },
            Reviews = new ReviewModel()
            {
                ReviewPeriod = source.ReviewPeriod,
                EmployerReviews = source.EmployerReviews,
                EmployerStars = source.EmployerStars,
                EmployerRating = source.EmployerRating,
                ApprenticeReviews = source.ApprenticeReviews,
                ApprenticeStars = source.ApprenticeStars,
                ApprenticeRating = source.ApprenticeRating,
            },
            ShortlistId = source.ShortlistId
        };
    }
}
