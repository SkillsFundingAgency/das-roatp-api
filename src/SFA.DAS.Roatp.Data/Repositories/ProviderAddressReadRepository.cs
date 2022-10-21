using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories;

[ExcludeFromCodeCoverage]
internal class ProviderAddressReadRepository : IProviderAddressReadRepository
{
    private readonly RoatpDataContext _roatpDataContext;

    public ProviderAddressReadRepository(RoatpDataContext roatpDataContext)
    {
        _roatpDataContext = roatpDataContext;
    }

    public async Task<List<ProviderAddress>> GetAllProviderAddresses()
    {
        return await _roatpDataContext.ProviderAddresses.AsNoTracking().ToListAsync();
    }
}