using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Shortlists.Commands.CreateShortlist;

public class CreateShortlistCommandValidator : AbstractValidator<CreateShortlistCommand>
{
    public const string InvalidUkprnLarsCodeMessage = "Provider should have a valid standard and location";
    public static readonly string ShortlistMaximumCapacityReachedMessaged = $"User has exceeded the maximum shortlist allowance of {MaximumShortlistAllowance}";
    public const int MaximumShortlistAllowance = 50;

    public CreateShortlistCommandValidator(
        IProviderRegistrationDetailsReadRepository providerRegistrationDetailsReadRepository,
        IStandardsReadRepository standardsReadRepository,
        IProvidersReadRepository providersReadRepository,
        IShortlistWriteRepository shortlistWriteRepository)
    {
        Include(new LarsCodeValidator(standardsReadRepository));

        Include(new UkprnValidator(providersReadRepository));

        RuleFor(s => s.Ukprn).MustAsync(async (model, ukprn, cancellation) =>
        {
            return await providerRegistrationDetailsReadRepository.IsMainActiveProvider(model.Ukprn, model.LarsCode);
        }).WithMessage(InvalidUkprnLarsCodeMessage);

        RuleFor(s => s.UserId)
            .NotEmpty()
            .MustAsync(async (userId, cancellationToken) =>
            {
                int count = await shortlistWriteRepository.GetShortlistCount(userId, cancellationToken);
                return count <= MaximumShortlistAllowance;
            })
            .WithMessage(ShortlistMaximumCapacityReachedMessaged);

        When(command => !string.IsNullOrWhiteSpace(command.LocationDescription), () =>
        {
            RuleFor(command => command.Latitude).NotEmpty().InclusiveBetween(NationalLatLong.MinimumLatitude, NationalLatLong.MaximumLatitude);
            RuleFor(command => command.Longitude).NotEmpty().InclusiveBetween(NationalLatLong.MinimumLongitude, NationalLatLong.MaximumLongitude);
        });
    }
}
