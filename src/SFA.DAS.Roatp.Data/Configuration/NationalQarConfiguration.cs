using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Roatp.Data.Configuration;

[ExcludeFromCodeCoverage]
public class NationalQarConfiguration : IEntityTypeConfiguration<NationalQar>
{
    public void Configure(EntityTypeBuilder<NationalQar> builder)
    {
        builder.ToTable(nameof(NationalQar));
        builder.HasKey(p => p.TimePeriod);
        builder.Property(p => p.Leavers).IsRequired();
        builder.Property(p => p.AchievementRate).IsRequired();
        builder.Property(p => p.CreatedDate).IsRequired();
    }
}