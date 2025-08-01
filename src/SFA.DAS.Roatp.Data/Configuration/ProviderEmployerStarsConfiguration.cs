using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.Configuration;

[ExcludeFromCodeCoverage]
public class ProviderEmployerStarsConfiguration : IEntityTypeConfiguration<ProviderEmployerStars>
{
    public void Configure(EntityTypeBuilder<ProviderEmployerStars> builder)
    {
        builder.ToTable(nameof(ProviderEmployerStars));
        builder.HasKey(table => new
        {
            table.TimePeriod,
            table.Ukprn
        });
        builder.Property(c => c.Rating).HasComputedColumnSql();
    }
}
