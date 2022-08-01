using System.Linq;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.ProviderCourse
{
    public class PatchProviderCourseCommand : IRequest, IUkprn, ILarsCode
    {
        private const string Replace = "Replace";
        public const string IsApprovedByRegulatorIdentifier = "IsApprovedByRegulator";
        public const string StandardInfoUrlIdentifier = "StandardInfoUrl";
        public const string ContactUsPhoneNumberIdentifier = "ContactUsPhoneNumber";
        private const string ContactUsEmailIdentifier = "ContactUsEmail";
        public const string ContactUsPageUrlIdentifier = "ContactUsPageUrl";

        public int Ukprn { get; set; }
        public int LarsCode { get; set; }
        public JsonPatchDocument<PatchProviderCourse> Patch { get; set; }

        public string StandardInfoUrl =>
            Patch.Operations.FirstOrDefault(x =>
                x.path == StandardInfoUrlIdentifier)?.value.ToString();

        public string ContactUsPhoneNumber =>
            Patch.Operations.FirstOrDefault(x =>
                x.path == ContactUsPhoneNumberIdentifier)?.value.ToString();

        public string ContactUsEmail =>
            Patch.Operations.FirstOrDefault(x =>
                x.path == ContactUsEmailIdentifier)?.value.ToString();

        public string ContactUsPageUrl =>
            Patch.Operations.FirstOrDefault(x =>
                x.path == ContactUsPageUrlIdentifier)?.value.ToString();

        public bool? IsApprovedByRegulator
        {
            get
            {
                var isApprovedByRegulator = Patch.Operations.FirstOrDefault(x =>
                    x.path == IsApprovedByRegulatorIdentifier)?.value;
                
                if (bool.TryParse(isApprovedByRegulator?.ToString(), out var result))
                    return result;

                return null;
            }
        }

        public bool IsPresentIsApprovedByRegulator =>
            Patch.Operations.Any(x =>
                x.path == IsApprovedByRegulatorIdentifier && x.op == Replace);

        public bool IsPresentStandardInfoUrl =>
            Patch.Operations.Any(x =>
                x.path == StandardInfoUrlIdentifier && x.op == Replace);

        public bool IsPresentContactUsPhoneNumber =>
            Patch.Operations.Any(x =>
                x.path == ContactUsPhoneNumberIdentifier && x.op == Replace);

        public bool IsPresentContactUsEmail =>
            Patch.Operations.Any(x =>
                x.path == ContactUsEmailIdentifier && x.op == Replace);

        public bool IsPresentContactUsPageUrl =>
            Patch.Operations.Any(x =>
                x.path == ContactUsPageUrlIdentifier && x.op == Replace);
    }
}
