using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data
{
    public class RoatpDataContext : DbContext
    {
        public DbSet<Provider> Providers { get; set; }
    }
}
