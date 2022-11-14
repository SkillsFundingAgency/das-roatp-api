using System;
using System.Linq;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.ProviderCourse
{
    public class PatchProviderCourseCommand : IRequest, IUkprn, ILarsCodeUkprn
    {
        private const string Replace = "replace";
        public const string IsApprovedByRegulatorIdentifier = "IsApprovedByRegulator";
        public const string StandardInfoUrlIdentifier = "StandardInfoUrl";
        public const string ContactUsPhoneNumberIdentifier = "ContactUsPhoneNumber";
        private const string ContactUsEmailIdentifier = "ContactUsEmail";
        public const string ContactUsPageUrlIdentifier = "ContactUsPageUrl";

        public int Ukprn { get; set; }
        public int LarsCode { get; set; }
        public JsonPatchDocument<PatchProviderCourse> Patch { get; set; }

        public string StandardInfoUrl =>
            Patch.Operations.FirstOrDefault(operation =>
                operation.path == StandardInfoUrlIdentifier && operation.op.Equals(Replace,StringComparison.CurrentCultureIgnoreCase))?.value.ToString();

        public string ContactUsPhoneNumber =>
            Patch.Operations.FirstOrDefault(operation =>
                operation.path == ContactUsPhoneNumberIdentifier && operation.op.Equals(Replace, StringComparison.CurrentCultureIgnoreCase))?.value.ToString();

        public string ContactUsEmail =>
            Patch.Operations.FirstOrDefault(operation =>
                operation.path == ContactUsEmailIdentifier && operation.op.Equals(Replace, StringComparison.CurrentCultureIgnoreCase))?.value.ToString();

        public string ContactUsPageUrl =>
            Patch.Operations.FirstOrDefault(operation =>
                operation.path == ContactUsPageUrlIdentifier && operation.op.Equals(Replace, StringComparison.CurrentCultureIgnoreCase))?.value.ToString();

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

        public bool IsPresentIsApprovedByRegulator =>
            Patch.Operations.Any(operation =>
                operation.path == IsApprovedByRegulatorIdentifier && operation.op.Equals(Replace, StringComparison.CurrentCultureIgnoreCase));

        public bool IsPresentStandardInfoUrl =>
            Patch.Operations.Any(operation =>
                operation.path == StandardInfoUrlIdentifier && operation.op.Equals(Replace, StringComparison.CurrentCultureIgnoreCase));

        public bool IsPresentContactUsPhoneNumber =>
            Patch.Operations.Any(operation =>
                operation.path == ContactUsPhoneNumberIdentifier && operation.op.Equals(Replace, StringComparison.CurrentCultureIgnoreCase));

        public bool IsPresentContactUsEmail =>
            Patch.Operations.Any(operation =>
                operation.path == ContactUsEmailIdentifier && operation.op.Equals(Replace, StringComparison.CurrentCultureIgnoreCase));

        public bool IsPresentContactUsPageUrl =>
            Patch.Operations.Any(operation =>
                operation.path == ContactUsPageUrlIdentifier && operation.op.Equals(Replace, StringComparison.CurrentCultureIgnoreCase));
    }
}
