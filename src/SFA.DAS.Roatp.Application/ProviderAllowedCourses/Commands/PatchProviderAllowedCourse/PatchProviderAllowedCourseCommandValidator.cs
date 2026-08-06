using System;
using System.Globalization;
using System.Linq;
using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

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
        RuleFor(x => x.PatchDoc.Operations)
            .MustAsync(async (command, ops, cancellationToken) =>
            {
                var lastDateStartsOp = ops.FirstOrDefault(o =>
                    string.Equals(o.path?.TrimStart('/'), nameof(PatchProviderAllowedCourseModel.LastDateStarts), StringComparison.OrdinalIgnoreCase));

                if (lastDateStartsOp == null)
                {
                    return true;
                }

                if (lastDateStartsOp.value is null)
                {
                    return true;
                }

                if (!DateTime.TryParse(
                        Convert.ToString(lastDateStartsOp.value, CultureInfo.InvariantCulture),
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.RoundtripKind,
                        out var patchedDate))
                {
                    return false;
                }

                var standard = await standardsReadRepository.GetStandard(command.LarsCode);

                return standard?.LastDateStarts == null
                       || patchedDate <= standard.LastDateStarts.Value;
            })
            .WithMessage(InvalidLastDateStarts);
    }
}
