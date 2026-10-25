using System.Linq;
using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.PutProviderAllowedCourse;

public class PutProviderAllowedCourseCommandValidator : AbstractValidator<PutProviderAllowedCourseCommand>
{
    public const string InvalidLastDateStarts = "LastDateStarts cannot be after LastDateStarts of the course";
    public const string NotExistsInProviderAllowedCourse = "Ukprn and LarsCode combination does not exist in provider allowed courses";
    public PutProviderAllowedCourseCommandValidator(IStandardsReadRepository standardsReadRepository, IProviderAllowedCoursesRepository providerAllowedCoursesRepository)
    {
        Include(new UserInfoValidator());

        RuleFor(pac => pac)
            .MustAsync(async (command, cancellation) =>
            {
                var providerAllowedCourses = await providerAllowedCoursesRepository.GetProviderAllowedCoursesByLarsCode(command.LarsCode, cancellation);
                return providerAllowedCourses.Any(p => p.Ukprn == command.Ukprn);
            })
            .WithMessage(NotExistsInProviderAllowedCourse);

        RuleFor(x => x.LastDateStarts)
            .MustAsync(async (command, lastDateStarts, cancellationToken) =>
            {
                var standard = await standardsReadRepository.GetStandard(command.LarsCode);

                return standard?.LastDateStarts == null || lastDateStarts?.Date <= standard.LastDateStarts.Value;
            })
            .When(x => x.LastDateStarts.HasValue)
            .WithMessage(InvalidLastDateStarts);
    }
}
