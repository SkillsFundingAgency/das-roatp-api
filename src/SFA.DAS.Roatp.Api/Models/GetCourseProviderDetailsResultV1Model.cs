using System;
using System.Collections.Generic;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetCourseProviderDetails;

public sealed class GetCourseProviderDetailsResultV1Model
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

    public static implicit operator GetCourseProviderDetailsResultV1Model(GetCourseProviderDetailsQueryResult source)
    {
        if (source == null)
        {
            return null;
        }
        return new GetCourseProviderDetailsResultV1Model
        {
            Ukprn = source.Ukprn,
            ProviderName = source.ProviderName,
            Address = new ShortProviderAddressModel()
            {
                AddressLine1 = source.Address.AddressLine1,
                AddressLine2 = source.Address.AddressLine2,
                AddressLine3 = source.Address.AddressLine3,
                AddressLine4 = source.Address.AddressLine4,
                Town = source.Address.Town,
                Postcode = source.Address.Postcode
            },
            Contact = new ContactModel()
            {
                MarketingInfo = source.Contact.MarketingInfo,
                Website = source.Contact.Website,
                Email = source.Contact.Email,
                PhoneNumber = source.Contact.PhoneNumber
            },
            CourseName = source.CourseName,
            Level = source.Level,
            LarsCode = int.TryParse(source.LarsCode, out var l) ? l : 0,
            IFateReferenceNumber = source.IFateReferenceNumber,
            QAR = new QarModel()
            {
                Period = source.QAR.Period,
                Leavers = source.QAR.Leavers,
                AchievementRate = source.QAR.AchievementRate,
                NationalLeavers = source.QAR.NationalLeavers,
                NationalAchievementRate = source.QAR.NationalAchievementRate
            },
            Reviews = new ReviewModel()
            {
                ReviewPeriod = source.Reviews.ReviewPeriod,
                EmployerReviews = source.Reviews.EmployerReviews,
                EmployerStars = source.Reviews.EmployerStars,
                EmployerRating = source.Reviews.EmployerRating,
                ApprenticeReviews = source.Reviews.ApprenticeReviews,
                ApprenticeStars = source.Reviews.ApprenticeStars,
                ApprenticeRating = source.Reviews.ApprenticeRating,
            },
            Locations = source.Locations,
            ShortlistId = source.ShortlistId
        };
    }
}
