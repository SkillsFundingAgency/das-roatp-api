using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.Configuration;

[ExcludeFromCodeCoverage]
public class ProviderCourseForecastConfiguration : IEntityTypeConfiguration<ProviderCourseForecast>
{
    public void Configure(EntityTypeBuilder<ProviderCourseForecast> builder)
    {
        builder.ToTable(nameof(ProviderCourseForecast));
    }
}
