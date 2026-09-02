using System.Linq;
using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.UpsertProviderAllowedCourse;

public class UpsertProviderAllowedCourseCommandValidator : AbstractValidator<UpsertProviderAllowedCourseCommand>
{
    public const string InvalidLastDateStarts = "LastDateStarts cannot be after LastDateStarts of the course";
    public const string ExistsInProviderAllowedCourse = "Course already exists in provider allowed course for provider";
    public UpsertProviderAllowedCourseCommandValidator(IStandardsReadRepository standardsReadRepository, IProviderAllowedCoursesRepository providerAllowedCoursesRepository)
    {
        Include(new UserInfoValidator());
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
        RuleFor(x => x)
            .MustAsync(async (command, cancellation) =>
            {
                if (!command.IsStartRestricted)
                {
                    return true;
                }

                var providerAllowedCourse = await providerAllowedCoursesRepository.GetProviderAllowedCoursesByLarsCode(command.LarsCode, cancellation);

                return !providerAllowedCourse.Any(x => x.Ukprn == command.Ukprn);
            })
            .WithMessage(ExistsInProviderAllowedCourse);
    }
}
