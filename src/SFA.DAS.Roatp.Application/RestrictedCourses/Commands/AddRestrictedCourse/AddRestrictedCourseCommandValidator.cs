using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.RestrictedCourses.Commands.AddRestrictedCourse;

public class AddRestrictedCourseCommandValidator : AbstractValidator<AddRestrictedCourseCommand>
{
    public const string LarsCodeAlreadyRestricted = "Lars code is already restricted";

    public AddRestrictedCourseCommandValidator(IStandardsReadRepository standardsReadRepository, IRestrictedCourseViewRepository restrictedCourseViewRepository)
    {
        Include(new UserInfoValidator());

        Include(new LarsCodeValidator(standardsReadRepository));

        RuleFor(x => x.LarsCode)
                .MustAsync(async (larsCode, cancellation) =>
                {
                    List<RestrictedCourseView> restrictedCourses = await restrictedCourseViewRepository.GetRestrictedCourses(cancellation);
                    return !restrictedCourses.Any(x => x.LarsCode == larsCode);
                })
                .WithMessage(LarsCodeAlreadyRestricted);
    }
}
