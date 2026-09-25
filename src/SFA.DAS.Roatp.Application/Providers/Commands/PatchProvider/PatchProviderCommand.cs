using System;
using System.Linq;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.Providers.Commands.PatchProvider
{
    public class PatchProviderCommand : IRequest<ValidatedResponse<Unit>>, IUkprn, IUserInfo
    {
        private const string Replace = "replace";
        public const string MarketingInfoIdentifier = "MarketingInfo";

        public int Ukprn { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public JsonPatchDocument<Domain.Models.PatchProvider> Patch { get; set; }

        public string MarketingInfo =>
            Patch.Operations.FirstOrDefault(operation =>
                operation.path == MarketingInfoIdentifier && operation.op.Equals(Replace, StringComparison.CurrentCultureIgnoreCase))?.value.ToString();


        public bool IsPresentMarketingInfo =>
            Patch.Operations.Any(operation =>
                operation.path == MarketingInfoIdentifier && operation.op.Equals(Replace, StringComparison.CurrentCultureIgnoreCase));

    }
}
