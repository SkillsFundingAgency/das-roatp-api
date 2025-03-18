using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Shortlists.Commands.DeleteExpiredShortlists;

public class DeleteExpiredShortlistsCommandHandler(IShortlistsRepository _shortlistsRepository) : IRequestHandler<DeleteExpiredShortlistsCommand>
{
    public Task Handle(DeleteExpiredShortlistsCommand request, CancellationToken cancellationToken)
    {
        return _shortlistsRepository.DeleteExpiredShortlistItems(Constants.ShortlistExpiryDays, cancellationToken);
    }
}
