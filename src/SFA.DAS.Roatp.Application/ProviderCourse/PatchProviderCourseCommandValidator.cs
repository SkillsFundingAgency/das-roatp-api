using FluentValidation;

namespace SFA.DAS.Roatp.Application.ProviderCourse
{
    public class PatchProviderCourseCommandValidator : AbstractValidator<PatchProviderCourseCommand>
    {
        public PatchProviderCourseCommandValidator()
        {
            RuleFor(c => c.Ukprn)
                .GreaterThan(10000000)
                .LessThan(99999999);
            RuleFor(c => c.LarsCode)
                .GreaterThan(0);
            RuleFor(c => c.Patch)
                .Must(x => x.Operations.Count > 0);
        }
    }
}
