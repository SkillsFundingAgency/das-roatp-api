using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Shortlists.Commands.DeleteShortlist;

public class DeleteShortlistCommandHandler(IShortlistsRepository shortlistsRepository) : IRequestHandler<DeleteShortlistCommand, DeleteShortlistCommandResult>
{
    public async Task<DeleteShortlistCommandResult> Handle(DeleteShortlistCommand request, CancellationToken cancellationToken)
    {
        var rows = await shortlistsRepository.Delete(request.ShortlistId, cancellationToken);
        return new(rows > 0);
    }
}
