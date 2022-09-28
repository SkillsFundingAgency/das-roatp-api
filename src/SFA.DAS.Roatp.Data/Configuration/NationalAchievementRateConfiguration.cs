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

            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.Ukprn).IsRequired(); 
            builder.Property(x => x.Age).IsRequired();
            builder.Property(x => x.SectorSubjectArea).HasMaxLength(1000).IsRequired();
            builder.Property(x => x.ApprenticeshipLevel).IsRequired();
            builder.Property(x => x.OverallCohort).IsRequired(false);
            builder.Property(x => x.OverallAchievementRate).IsRequired(false);
        }
    }
}