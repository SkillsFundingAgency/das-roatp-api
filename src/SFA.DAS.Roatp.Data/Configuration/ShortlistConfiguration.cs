using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.Configuration;

[ExcludeFromCodeCoverage]
public class ShortlistConfiguration : IEntityTypeConfiguration<Shortlist>
{
    public void Configure(EntityTypeBuilder<Shortlist> builder)
    {
        builder.ToTable(nameof(Shortlist));
        builder.Property(p => p.Latitude).HasColumnType("float");
        builder.Property(p => p.Longitude).HasColumnType("float");
    }
}
