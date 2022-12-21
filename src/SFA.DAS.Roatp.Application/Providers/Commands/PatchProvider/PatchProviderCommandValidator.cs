using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Providers.Commands.PatchProvider
{
    public class PatchProviderCommandValidator : AbstractValidator<PatchProviderCommand>
    {
        public const string Replace = "replace";
        public const string NoPatchOperationsPresentErrorMessage = "There are no patch operations in this call";
        public const string PatchOperationContainsUnavailableFieldErrorMessage = "This patch operation contains an unexpected field and will not continue";
        public const string PatchOperationContainsUnavailableOperationErrorMessage = "This patch operation contains an unexpected operation and will not continue";

        public const string IsApprovedByRegulatorIsNotABooleanErrorMessage = "The patch contains an update for IsApprovedByRegulator that is not a boolean value";

        public static readonly IList<string> PatchFields = new ReadOnlyCollection<string>(
                new List<string>
                {
                    "MarketingInfo",
                });

        public PatchProviderCommandValidator(IProvidersReadRepository providersReadRepository)
        {
            Include(new UkprnValidator(providersReadRepository));

            RuleFor(c => c.Patch.Operations.Count).GreaterThan(0).WithMessage(NoPatchOperationsPresentErrorMessage);

            RuleFor(c => c.Patch.Operations.Count(operation => !PatchFields.Contains(operation.path)))
                .Equal(0)
                .WithMessage(PatchOperationContainsUnavailableFieldErrorMessage);

            RuleFor(c => c.Patch.Operations.Count(operation => !operation.op.Equals(Replace, StringComparison.CurrentCultureIgnoreCase)))
                .Equal(0)
                .WithMessage(PatchOperationContainsUnavailableOperationErrorMessage);

            RuleFor(c => c.MarketingInfo)
                .NotEmpty()
                .MaximumLength(750)
                .When(c => c.IsPresentMarketingInfo);
        }
    }
}
