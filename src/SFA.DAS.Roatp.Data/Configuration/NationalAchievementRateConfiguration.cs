using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Roatp.Data.Configuration
{
    [ExcludeFromCodeCoverage]
    public class NationalAchievementRateConfiguration : IEntityTypeConfiguration<NationalAchievementRate>
    {
        public void Configure(EntityTypeBuilder<NationalAchievementRate> builder)
        {
            builder.ToTable(nameof(NationalAchievementRate));
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).UseIdentityColumn().IsRequired().HasColumnName("Id");
            builder.Property(x => x.Ukprn).HasColumnName("Ukprn").HasColumnType("int");
            builder.Property(x => x.Age).HasColumnName("Age").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.SectorSubjectArea).HasColumnName("SectorSubjectArea").HasColumnType("varchar").HasMaxLength(1000).IsRequired();
            builder.Property(x => x.ApprenticeshipLevel).HasColumnName("ApprenticeshipLevel").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.OverallCohort).HasColumnName("OverallCohort").HasColumnType("int").IsRequired(false);
            builder.Property(x => x.OverallAchievementRate).HasColumnName("OverallAchievementRate").HasColumnType("decimal").IsRequired(false);
        }
    }
}