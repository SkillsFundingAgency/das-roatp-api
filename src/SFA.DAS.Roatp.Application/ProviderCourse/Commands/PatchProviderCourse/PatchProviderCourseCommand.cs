using System;
using System.Linq;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Commands.PatchProviderCourse;

public class PatchProviderCourseCommand : IRequest<ValidatedResponse<bool>>, IUkprn, ILarsCodeUkprn, IUserInfo
{
    private const string Replace = "replace";
    public const string IsApprovedByRegulatorIdentifier = "IsApprovedByRegulator";
    public const string StandardInfoUrlIdentifier = "StandardInfoUrl";
    public const string ContactUsPhoneNumberIdentifier = "ContactUsPhoneNumber";
    private const string ContactUsEmailIdentifier = "ContactUsEmail";
    private const string HasOnlineDeliveryOptionIdentifier = "HasOnlineDeliveryOption";

    public int Ukprn { get; set; }
    public string LarsCode { get; set; }
    public string UserId { get; set; }
    public string UserDisplayName { get; set; }
    public JsonPatchDocument<Domain.Models.PatchProviderCourse> Patch { get; set; }

    public string StandardInfoUrl =>
        Patch.Operations.FirstOrDefault(operation =>
            operation.path == StandardInfoUrlIdentifier && operation.op.Equals(Replace, StringComparison.CurrentCultureIgnoreCase))?.value.ToString();

    public string ContactUsPhoneNumber =>
        Patch.Operations.FirstOrDefault(operation =>
            operation.path == ContactUsPhoneNumberIdentifier && operation.op.Equals(Replace, StringComparison.CurrentCultureIgnoreCase))?.value.ToString();

    public string ContactUsEmail =>
        Patch.Operations.FirstOrDefault(operation =>
            operation.path == ContactUsEmailIdentifier && operation.op.Equals(Replace, StringComparison.CurrentCultureIgnoreCase))?.value.ToString();

    public bool? IsApprovedByRegulator
    {
        get
        {
            var isApprovedByRegulator = Patch.Operations.FirstOrDefault(operation =>
                operation.path == IsApprovedByRegulatorIdentifier && operation.op.Equals(Replace, StringComparison.CurrentCultureIgnoreCase))?.value;

            if (bool.TryParse(isApprovedByRegulator?.ToString(), out var result))
                return result;

            return null;
        }
    }

    public bool? HasOnlineDeliveryOption
    {
        get
        {
            var hasOnlineDeliveryOption = Patch.Operations.FirstOrDefault(operation =>
                operation.path == HasOnlineDeliveryOptionIdentifier && operation.op.Equals(Replace, StringComparison.CurrentCultureIgnoreCase))?.value;

            if (bool.TryParse(hasOnlineDeliveryOption?.ToString(), out var result))
                return result;

            return null;
        }
    }

    public bool IsPresentIsApprovedByRegulator =>
        Patch.Operations.Any(operation =>
            operation.path == IsApprovedByRegulatorIdentifier && operation.op.Equals(Replace, StringComparison.CurrentCultureIgnoreCase));

    public bool IsPresentHasOnlineDeliveryOption =>
        Patch.Operations.Any(operation =>
            operation.path == HasOnlineDeliveryOptionIdentifier && operation.op.Equals(Replace, StringComparison.CurrentCultureIgnoreCase));

    public bool IsPresentStandardInfoUrl =>
        Patch.Operations.Any(operation =>
            operation.path == StandardInfoUrlIdentifier && operation.op.Equals(Replace, StringComparison.CurrentCultureIgnoreCase));

    public bool IsPresentContactUsPhoneNumber =>
        Patch.Operations.Any(operation =>
            operation.path == ContactUsPhoneNumberIdentifier && operation.op.Equals(Replace, StringComparison.CurrentCultureIgnoreCase));

    public bool IsPresentContactUsEmail =>
        Patch.Operations.Any(operation =>
            operation.path == ContactUsEmailIdentifier && operation.op.Equals(Replace, StringComparison.CurrentCultureIgnoreCase));
}