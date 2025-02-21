using System;
using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.Shortlists.Commands.CreateShortlist;

public class CreateShortlistCommand : IRequest<ValidatedResponse<CreateShortlistCommandResult>>, IUkprn, ILarsCode
{
    public Guid UserId { get; set; }
    public int Ukprn { get; set; }
    public int LarsCode { get; set; }
    public string LocationDescription { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}
