using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories;

[ExcludeFromCodeCoverage]
internal class NationalQarReadRepository : INationalQarReadRepository
{
    private readonly RoatpDataContext _roatpDataContext;

    public NationalQarReadRepository(RoatpDataContext roatpDataContext)
    {
        _roatpDataContext = roatpDataContext;
    }


    public async Task<List<NationalQar>> GetAll()
    {
        return await _roatpDataContext.NationalQars.ToListAsync();
    }

}