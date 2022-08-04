using MediatR;
using SFA.DAS.Roatp.Application.Common;
using System;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.Delete
{
    public class DeleteProviderCourseLocationCommand : IRequest<Unit>, ILarsCode, IUkprn
    {
        public int Ukprn { get; }
        public int LarsCode { get; }
        public Guid Id { get; set; }
        public string UserId { get; }
        public DeleteProviderCourseLocationCommand(int ukprn, int larsCode, Guid id, string userId)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
            Id = id;
            UserId = userId;
        }
    }
}
