using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.ProviderCourse
{
    public class PatchProviderCourseCommand : IRequest, IUkprn, ILarsCode
    {
        public int Ukprn { get; set; }
        public int LarsCode { get; set; }
        public JsonPatchDocument<PatchProviderCourse> Patch { get; set; }

    }
}
