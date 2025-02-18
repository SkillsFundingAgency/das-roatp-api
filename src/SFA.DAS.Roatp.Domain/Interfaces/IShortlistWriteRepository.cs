using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces;

public interface IShortlistWriteRepository
{
    Task<Shortlist> Get(Guid userId, int ukprn, int larsCode, string locationDescription, CancellationToken cancellationToken);

    Task Create(Shortlist shortlist, CancellationToken cancellationToken);
}
