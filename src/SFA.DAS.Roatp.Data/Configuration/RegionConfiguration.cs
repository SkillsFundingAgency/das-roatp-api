using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Roatp.Data.Configuration
{
    [ExcludeFromCodeCoverage]
    public class RegionConfiguration : IEntityTypeConfiguration<Region>
    {
        public void Configure(EntityTypeBuilder<Region> builder)
        {
            builder.ToTable(nameof(Region));
            builder.HasKey(p => p.Id);
            builder.Property(p => p.SubregionName).IsRequired().HasMaxLength(250);
            builder.Property(p => p.RegionName).IsRequired().HasMaxLength(25);
            builder.Property(p => p.Latitude).HasColumnType("float").IsRequired();
            builder.Property(p => p.Longitude).HasColumnType("float").IsRequired();
        }
    }
}
