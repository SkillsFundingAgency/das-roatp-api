using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Shortlists.Commands.CreateShortlist;

public class CreateShortlistCommandHandler(IShortlistWriteRepository _shortlistWriteRepository) : IRequestHandler<CreateShortlistCommand, ValidatedResponse<CreateShortlistCommandResult>>
{
    public async Task<ValidatedResponse<CreateShortlistCommandResult>> Handle(CreateShortlistCommand request, CancellationToken cancellationToken)
    {
        Shortlist shortlist = await _shortlistWriteRepository.Get(request.UserId, request.Ukprn, request.LarsCode, request.LocationDescription, cancellationToken);

        if (shortlist == null)
        {
            shortlist = ConvertToShortlist(request);
            await _shortlistWriteRepository.Create(shortlist, cancellationToken);
        }

        return new ValidatedResponse<CreateShortlistCommandResult>(new CreateShortlistCommandResult(shortlist));
    }

    private static Shortlist ConvertToShortlist(CreateShortlistCommand command)
        => new()
        {
            Id = Guid.NewGuid(),
            UserId = command.UserId,
            Ukprn = command.Ukprn,
            LarsCode = command.LarsCode,
            LocationDescription = command.LocationDescription,
            Latitude = command.Latitude,
            Longitude = command.Longitude
        };
}
