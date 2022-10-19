using FluentValidation;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProvidersCountForCourse
{
    public class GetProvidersCountForCourseQueryValidator : AbstractValidator<GetProvidersCountForCourseQuery>
    {
        public const string InvalidLarsCodeErrorMessage = "Invalid larsCode";
        public GetProvidersCountForCourseQueryValidator(IStandardsReadRepository standardsReadRepository)
        {
            RuleFor(x => x.LarsCode)
               .Cascade(CascadeMode.Stop)
               .GreaterThan(0)
               .WithMessage(InvalidLarsCodeErrorMessage)
               .MustAsync(async (model, larsCode, cancellation) =>
               {
                   var standard = await standardsReadRepository.GetStandard(larsCode);
                   return standard != null;
               })
               .WithMessage(InvalidLarsCodeErrorMessage);
        }
    }
}
