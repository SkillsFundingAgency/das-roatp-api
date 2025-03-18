using FluentValidation;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetCourseTrainingProvidersCount;

public sealed class GetCourseTrainingProvidersCountValidator : AbstractValidator<GetCourseTrainingProvidersCountQuery>
{
    public const string DistanceValidationMessage = "Distance should always be set when Longitude and Latitude have values.";

    public const string LarsCodesValidationMessage = "At least one lars code must be provided.";

    public GetCourseTrainingProvidersCountValidator()
    {
        Include(new CoordinatesValidator());

        RuleFor(p => p.LarsCodes)
            .NotEmpty()
            .WithMessage(LarsCodesValidationMessage);

        RuleFor(p => p.Distance)
            .NotNull()
            .When(p => p.Latitude.HasValue && p.Longitude.HasValue)
            .WithMessage(DistanceValidationMessage);
    }
}
