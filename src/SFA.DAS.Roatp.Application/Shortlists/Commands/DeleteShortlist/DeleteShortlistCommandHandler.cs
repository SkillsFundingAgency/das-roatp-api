using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Shortlists.Commands.DeleteShortlist;

public class DeleteShortlistCommandHandler(IShortlistsRepository shortlistsRepository) : IRequestHandler<DeleteShortlistCommand>
{
    public Task Handle(DeleteShortlistCommand request, CancellationToken cancellationToken)
        => shortlistsRepository.Delete(request.ShortlistId, cancellationToken);
}
