using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Commands.DeleteProviderCourse
{
    public class DeleteProviderCourseCommandValidator : AbstractValidator<DeleteProviderCourseCommand>
    {
        public DeleteProviderCourseCommandValidator(IProvidersReadRepository providersReadRepository, IProviderCoursesReadRepository providerCoursesReadRepository)
        {
            Include(new UkprnValidator(providersReadRepository));

            Include(new LarsCodeValidator(providersReadRepository, providerCoursesReadRepository));

            RuleFor(c => c.UserId).NotEmpty();
        }
    }
}
