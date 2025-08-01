using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.Configuration;

public class ProviderApprenticeStarsConfiguration : IEntityTypeConfiguration<ProviderApprenticeStars>
{
    public void Configure(EntityTypeBuilder<ProviderApprenticeStars> builder)
    {
        builder.ToTable(nameof(ProviderApprenticeStars));
        builder.HasKey(table => new
        {
            table.TimePeriod,
            table.Ukprn
        });
        builder.Property(c => c.Rating).HasComputedColumnSql();
    }
}
