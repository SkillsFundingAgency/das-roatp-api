using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetCourseProviderDetails;

public sealed class GetCourseProviderDetailsQueryHandler(ICourseProviderDetailsReadRepository _courseProviderDetailsReadRepository) : IRequestHandler<GetCourseProviderDetailsQuery, ValidatedResponse<GetCourseProviderDetailsQueryResult>>
{
    public async Task<ValidatedResponse<GetCourseProviderDetailsQueryResult>> Handle(GetCourseProviderDetailsQuery query, CancellationToken cancellationToken)
    {
        var providerDetails = await _courseProviderDetailsReadRepository.GetCourseProviderDetails(
            new GetCourseProviderDetailsParameters()
            {
                LarsCode = query.LarsCode,
                Ukprn = query.Ukprn,
                Lat = query.Lat,
                Lon = query.Lon,
                Location = query.Location,
                UserId = query.UserId
            },
            cancellationToken
        );

        if(providerDetails.Count < 1)
        {
            return new ValidatedResponse<GetCourseProviderDetailsQueryResult>((GetCourseProviderDetailsQueryResult)null);
        }

        var provider = providerDetails[0];

        var result = new GetCourseProviderDetailsQueryResult()
        {
            Ukprn = provider.Ukprn,
            ProviderName = provider.ProviderName,
            Address = new ShortProviderAddressModel()
            {
                AddressLine1 = provider.MainAddressLine1,
                AddressLine2 = provider.MainAddressLine2,
                AddressLine3 = provider.MainAddressLine3,
                AddressLine4 = provider.MainAddressLine4,
                Town = provider.MainTown,
                Postcode = provider.MainPostcode
            },
            Contact = new ContactModel()
            {
                MarketingInfo = provider.MarketingInfo,
                Website = provider.Website,
                Email = provider.Email,
                PhoneNumber = provider.PhoneNumber
            },
            CourseName = provider.CourseName,
            Level = provider.Level,
            LarsCode = provider.LarsCode,
            IFateReferenceNumber = provider.IFateReferenceNumber,
            QAR = new QarModel()
            {
                Period = provider.Period,
                Leavers = provider.Leavers,
                AchievementRate = provider.AchievementRate,
                NationalLeavers = provider.NationalLeavers,
                NationalAchievementRate = provider.NationalAchievementRate
            },
            Reviews = new ReviewModel()
            {
                ReviewPeriod = provider.ReviewPeriod,
                EmployerReviews = provider.EmployerReviews,
                EmployerStars = provider.EmployerStars,
                EmployerRating = provider.EmployerRating,
                ApprenticeReviews = provider.ApprenticeReviews,
                ApprenticeStars = provider.ApprenticeStars,
                ApprenticeRating = provider.ApprenticeRating,
            },
            ShortlistId = provider.ShortlistId,
            Locations = providerDetails.Select(model => new LocationModel
            {
                Ordering = model.Ordering,
                AtEmployer = model.AtEmployer,
                BlockRelease = model.BlockRelease,
                DayRelease = model.DayRelease,
                LocationType = model.LocationType,
                CourseLocation = model.CourseLocation,
                AddressLine1 = model.AddressLine1,
                AddressLine2 = model.AddressLine2,
                Town = model.Town,
                County = model.County,
                Postcode = model.Postcode,
                CourseDistance = model.CourseDistance
            })
        };

        return new ValidatedResponse<GetCourseProviderDetailsQueryResult>(result);
    }
}
