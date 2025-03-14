using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.UnitTests.Setup;

public static class RoatpDataContextFactory
{
    public static RoatpDataContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<RoatpDataContext>()
            .UseInMemoryDatabase(databaseName: $"{nameof(RoatpDataContext)}_" + Guid.NewGuid().ToString())
            .Options;

        return new RoatpDataContext(options);
    }

    public static void SeedProviderRegistrationDetails(this RoatpDataContext context, IEnumerable<ProviderRegistrationDetail> registrations)
    {
        context.AddRange(registrations);
        context.SaveChanges();
    }
}
