using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Shortlists.Commands.CreateShortlist;

public class CreateShortlistCommandHandler(IShortlistsRepository _shortlistWriteRepository) : IRequestHandler<CreateShortlistCommand, ValidatedResponse<CreateShortlistCommandResult>>
{
    public async Task<ValidatedResponse<CreateShortlistCommandResult>> Handle(CreateShortlistCommand request, CancellationToken cancellationToken)
    {
        CreateShortlistCommandResult result = new();

        Shortlist shortlist = await _shortlistWriteRepository.Get(request.UserId, request.Ukprn, request.LarsCode, request.LocationName, cancellationToken);

        if (shortlist == null)
        {
            shortlist = ConvertToShortlist(request);
            result.IsCreated = true;
            await _shortlistWriteRepository.Create(shortlist, cancellationToken);
        }

        result.ShortlistId = shortlist.Id;
        return new ValidatedResponse<CreateShortlistCommandResult>(result);
    }

    private static Shortlist ConvertToShortlist(CreateShortlistCommand command)
        => new()
        {
            Id = Guid.NewGuid(),
            UserId = command.UserId,
            Ukprn = command.Ukprn,
            LarsCode = command.LarsCode,
            LocationName = command.LocationName,
            Latitude = string.IsNullOrEmpty(command.LocationName) ? null : command.Latitude,
            Longitude = string.IsNullOrEmpty(command.LocationName) ? null : command.Longitude,
            CreatedDate = DateTime.UtcNow
        };
}
