using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse
{
    public class GetProviderDetailsForCourseQueryValidator : AbstractValidator<GetProviderDetailsForCourseQuery>
    {
        public GetProviderDetailsForCourseQueryValidator(IProvidersReadRepository providersReadRepository, IStandardsReadRepository standardsReadReadRepository)
        {
            Include(new CoordinatesValidator());
            Include(new LarsCodeValidator(standardsReadReadRepository));
            Include(new UkprnValidator(providersReadRepository));
        }
    }
}