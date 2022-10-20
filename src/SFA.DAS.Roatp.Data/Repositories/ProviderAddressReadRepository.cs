using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories;

[ExcludeFromCodeCoverage]
internal class ProviderAddressReadRepository : IProviderAddressReadRepository
{
    private readonly RoatpDataContext _roatpDataContext;
    private readonly ILogger<ProviderAddressReadRepository> _logger;

    public ProviderAddressReadRepository(RoatpDataContext roatpDataContext, ILogger<ProviderAddressReadRepository> logger)
    {
        _roatpDataContext = roatpDataContext;
        _logger = logger;
    }

    public async Task<List<ProviderAddress>> GetAllProviderAddresses()
    {
        return await _roatpDataContext.ProviderAddresses.AsNoTracking().ToListAsync();
    }
}