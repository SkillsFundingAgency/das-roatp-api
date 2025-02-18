using System;
using MediatR;

namespace SFA.DAS.Roatp.Application.Shortlists.Commands.CreateShortlist;

public class CreateShortlistCommand : IRequest<CreateShortlistCommandResult>
{
    public Guid UserId { get; set; }
    public int Ukprn { get; set; }
    public int LarsCode { get; set; }
    public string LocationDescription { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}
