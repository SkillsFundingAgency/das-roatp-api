using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.Configuration
{
    [ExcludeFromCodeCoverage]
    public class NationalAchievementRateConfiguration : IEntityTypeConfiguration<NationalAchievementRate>
    {
        public void Configure(EntityTypeBuilder<NationalAchievementRate> builder)
        {
            builder.ToTable(nameof(NationalAchievementRate));
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.Age).IsRequired().HasConversion<int>();
            builder.Property(x => x.ApprenticeshipLevel).IsRequired().HasConversion<int>();
            builder.Property(x => x.OverallCohort).IsRequired(false);
            builder.Property(x => x.OverallAchievementRate).HasColumnType("decimal").IsRequired(false);
        }
    }
}