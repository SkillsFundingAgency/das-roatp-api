using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Commands.PatchProviderCourse;

public class PatchProviderCourseCommandValidator : AbstractValidator<PatchProviderCourseCommand>
{
    public const string Replace = "replace";
    public const string NoPatchOperationsPresentErrorMessage = "There are no patch operations in this call";
    public const string PatchOperationContainsUnavailableFieldErrorMessage = "This patch operation contains an unexpected field and will not continue";
    public const string PatchOperationContainsUnavailableOperationErrorMessage = "This patch operation contains an unexpected operation and will not continue";

    public const string IsApprovedByRegulatorIsNotABooleanErrorMessage = "The patch contains an update for IsApprovedByRegulator that is not a boolean value";
    public const string HasOnlineDeliveryOptionIsNotABooleanErrorMessage = "The patch contains an update for HasOnlineDeliveryOption that is not a boolean value";

    public static readonly IList<string> PatchFields = new ReadOnlyCollection<string>(
        new List<string>
        {
            "ContactUsEmail",
            "ContactUsPhoneNumber",
            "StandardInfoUrl",
            "IsApprovedByRegulator",
            "HasOnlineDeliveryOption"
        });

    public PatchProviderCourseCommandValidator(IProvidersReadRepository providersReadRepository,
        IProviderCoursesReadRepository providerCoursesReadRepository)
    {
        Include(new UkprnValidator(providersReadRepository));

        Include(new LarsCodeUkprnCombinationValidator(providersReadRepository, providerCoursesReadRepository));

        RuleFor(c => c.Patch.Operations.Count).GreaterThan(0).WithMessage(NoPatchOperationsPresentErrorMessage);

        RuleFor(c => c.Patch.Operations.Count(operation => !PatchFields.Contains(operation.path)))
            .Equal(0)
            .WithMessage(PatchOperationContainsUnavailableFieldErrorMessage);

        RuleFor(c => c.Patch.Operations.Count(operation => !operation.op.Equals(Replace, StringComparison.CurrentCultureIgnoreCase)))
            .Equal(0)
            .WithMessage(PatchOperationContainsUnavailableOperationErrorMessage);

        RuleFor(c => c.IsApprovedByRegulator != null)
            .Equal(true)
            .When(c => c.IsPresentIsApprovedByRegulator)
            .WithMessage(IsApprovedByRegulatorIsNotABooleanErrorMessage);

        RuleFor(c => c.ContactUsEmail)
            .MustBeValidEmail()
            .When(c => c.IsPresentContactUsEmail);

        RuleFor(c => c.ContactUsPhoneNumber)
            .MustBeValidPhoneNumber()
            .When(c => c.IsPresentContactUsPhoneNumber);

        RuleFor(c => c.StandardInfoUrl)
            .MustBeValidUrl("Website")
            .When(c => c.IsPresentStandardInfoUrl);

        RuleFor(c => c.HasOnlineDeliveryOption)
            .NotEmpty()
            .When(c => c.IsPresentHasOnlineDeliveryOption)
            .WithMessage(HasOnlineDeliveryOptionIsNotABooleanErrorMessage);
    }
}