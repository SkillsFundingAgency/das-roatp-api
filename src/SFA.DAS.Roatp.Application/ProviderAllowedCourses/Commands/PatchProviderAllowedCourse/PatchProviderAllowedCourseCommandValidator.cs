using System.Linq;
using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.PatchProviderAllowedCourse;

public class PatchProviderAllowedCourseCommandValidator : AbstractValidator<PatchProviderAllowedCourseCommand>
{
    public const string InvalidLastDateStarts = "LastDateStarts cannot be after LastDateStarts of the course";
    public const string NotExistsInProviderAllowedCourse = "Ukprn and LarsCode combination does not exist in provider allowed courses";
    public PatchProviderAllowedCourseCommandValidator(IStandardsReadRepository standardsReadRepository, IProviderAllowedCoursesRepository providerAllowedCoursesRepository)
    {
        Include(new UserInfoValidator());
        RuleFor(pac => pac)
            .MustAsync(async (command, cancellation) =>
            {
                var providerAllowedCourses = await providerAllowedCoursesRepository.GetProviderAllowedCoursesByLarsCode(command.LarsCode, cancellation);
                return providerAllowedCourses.Any(p => p.Ukprn == command.Ukprn);
            })
            .WithMessage(NotExistsInProviderAllowedCourse);
        RuleFor(x => x)
            .MustAsync(async (command, cancellation) =>
            {
                if (!command.LastDateStarts.HasValue)
                {
                    return true;
                }
                var standard = await standardsReadRepository.GetStandard(command.LarsCode);
                return !command.LastDateStarts.HasValue
                       || standard?.LastDateStarts == null
                       || command.LastDateStarts.Value <= standard.LastDateStarts.Value;
            })
            .WithMessage(InvalidLastDateStarts);
    }
}
