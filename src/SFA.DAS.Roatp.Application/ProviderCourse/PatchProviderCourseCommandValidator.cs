﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourse
{
    public class PatchProviderCourseCommandValidator : AbstractValidator<PatchProviderCourseCommand>
    {
        public const string Replace = "replace";
        public const string NoPatchOperationsPresentErrorMessage = "There are no patch operations in this call";
        public const string PatchOperationContainsUnavailableFieldErrorMessage = "This patch operation contains an unexpected field and will not continue";
        public const string PatchOperationContainsUnavailableOperationErrorMessage = "This patch operation contains an unexpected operation and will not continue";

        public const string IsApprovedByRegulatorIsNotABooleanErrorMessage = "The patch contains an update for IsApprovedByRegulator that is not a boolean value";

        public static readonly IList<string> PatchFields = new ReadOnlyCollection<string>(
                new List<string>
                {
                    "ContactUsEmail",
                    "ContactUsPhoneNumber",
                    "ContactUsPageUrl",
                     "StandardInfoUrl",
                    "IsApprovedByRegulator"
                });

        public PatchProviderCourseCommandValidator(IProvidersReadRepository providerReadRepository,
            IProviderCourseReadRepository providerCourseReadRepository)
        {
            Include(new UkprnValidator(providerReadRepository));

            Include(new LarsCodeValidator(providerReadRepository, providerCourseReadRepository));

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

            RuleFor(c => c.ContactUsPageUrl)
                .MustBeValidUrl("Contact page")
                .When(c => c.IsPresentContactUsPageUrl);

            RuleFor(c => c.StandardInfoUrl)
                .MustBeValidUrl("Website")
                .When(c => c.IsPresentStandardInfoUrl);
        }
    }
}
