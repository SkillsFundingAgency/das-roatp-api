﻿using System.Linq;
using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Commands.CreateProviderCourse
{
    public class CreateProviderCourseCommandValidator : AbstractValidator<CreateProviderCourseCommand>
    {
        public const string RegulatedStandardMustBeApprovedMessage = "The standard is regulated and you must be approved by the regulator";
        public const string RegulatorsApprovalNotRequired = "This course is not regulated, IsApprovedByRegulator should be null";
        public const string EitherNationalOrRegionalMessage = "If the national delivery option is available, then the sub-regions are not required";
        public const string AtleastOneLocationIsRequiredMessage = "National delivery option is not set and there are no regions or provider locations either. Any one of these is required.";
        public const string LocationIdNotFoundMessage = "At least one of the location ids was not found";

        public CreateProviderCourseCommandValidator(
            IProviderReadRepository providerReadRepository,
            IStandardReadRepository standardReadRepository,
            IProviderCourseReadRepository providerCourseReadRepository,
            IProviderLocationsReadRepository providerLocationsReadRepository)
        {
            Include(new UkprnValidator(providerReadRepository));

            Include(new LarsCodeValidatorV2(standardReadRepository, providerCourseReadRepository, false));

            WhenAsync(
                async (command, _) => await IsStandardRegulated(command.LarsCode, standardReadRepository),
                () =>
                {
                    RuleFor(c => c.IsApprovedByRegulator)
                        .Equal(true)
                        .WithMessage(RegulatedStandardMustBeApprovedMessage);
                })
                .Otherwise(() => 
                {
                    RuleFor(c => c.IsApprovedByRegulator)
                        .Null()
                        .WithMessage(RegulatorsApprovalNotRequired);
                });

            When((command) => command.HasNationalDeliveryOption, () => 
            {
                RuleFor((c) => c.SubregionIds)
                    .Empty()
                    .WithMessage(EitherNationalOrRegionalMessage);
            })
            .Otherwise(() =>
            {
                When((command) => command.SubregionIds == null || command.SubregionIds.Count == 0, () =>
                {
                    RuleFor((c) => c.ProviderLocations)
                        .NotEmpty()
                        .WithMessage(AtleastOneLocationIsRequiredMessage)
                        .MustAsync(
                            async (command, providerLocations,cancellation) =>
                            {
                                var locations = await providerLocationsReadRepository.GetAllProviderLocations(command.Ukprn);
                                return !providerLocations.Any(providerLocation => locations.All(l => l.NavigationId != providerLocation.ProviderLocationId));
                            })
                        .WithMessage(LocationIdNotFoundMessage);
                });
            });

            RuleFor(c => c.ContactUsEmail)
                .NotEmpty()
                .WithMessage(c => ValidationMessages.IsRequired(nameof(c.ContactUsEmail)))
                .MustBeValidEmail();

            RuleFor(c => c.ContactUsPhoneNumber)
                .NotEmpty()
                .WithMessage(c => ValidationMessages.IsRequired(nameof(c.ContactUsPhoneNumber)))
                .MustBeValidPhoneNumber();

            RuleFor(c => c.ContactUsPageUrl)
                .NotEmpty()
                .WithMessage(c => ValidationMessages.IsRequired(nameof(c.ContactUsPageUrl)))
                .MustBeValidUrl("Contact page");

            RuleFor(c => c.StandardInfoUrl)
                .NotEmpty()
                .WithMessage(c => ValidationMessages.IsRequired(nameof(c.StandardInfoUrl)))
                .MustBeValidUrl("Website");

        }

        private async Task<bool> IsStandardRegulated(int larsCode, IStandardReadRepository standardReadRepository)
        {
            var standard = await standardReadRepository.GetStandard(larsCode);
            var result = !string.IsNullOrEmpty(standard?.ApprovalBody);
            return result;
        }
    }
}
