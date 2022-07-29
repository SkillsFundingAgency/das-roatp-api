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
        public const string IsApprovedByRegulator = "IsApprovedByRegulator";
        public const string StandardInfoUrl = "StandardInfoUrl";
        public const string ContactUsPhoneNumber = "ContactUsPhoneNumber";
        private const string ContactUsEmail = "ContactUsEmail";
        public const string ContactUsPageUrl = "ContactUsPageUrl";

        public int Ukprn { get; set; }
        public int LarsCode { get; set; }
        public JsonPatchDocument<PatchProviderCourse> Patch { get; set; }

        public bool IsPresentIsApprovedByRegulator =>
            Patch.Operations.Any(x =>
                x.path == IsApprovedByRegulator && x.op == Replace);

        public bool IsPresentStandardInfoUrl =>
            Patch.Operations.Any(x =>
                x.path == StandardInfoUrl && x.op == Replace);

        public bool IsPresentContactUsPhoneNumber =>
            Patch.Operations.Any(x =>
                x.path == ContactUsPhoneNumber && x.op == Replace);

        public bool IsPresentContactUsEmail =>
            Patch.Operations.Any(x =>
                x.path == ContactUsEmail && x.op == Replace);

        public bool IsPresentContactUsPageUrl =>
            Patch.Operations.Any(x =>
                x.path == ContactUsPageUrl && x.op == Replace);
    }
}
