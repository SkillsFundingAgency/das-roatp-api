using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

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
        builder.Property(p => p.ReviewCount).IsRequired();
        builder.Property(p => p.Stars).IsRequired();
        builder.Property(p => p.Rating).IsRequired();
    }
}
