using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.Configuration;

[ExcludeFromCodeCoverage]
public class ForecastQuartersConfiguration : IEntityTypeConfiguration<ForecastQuarter>
{
    public void Configure(EntityTypeBuilder<ForecastQuarter> builder)
    {
        builder.ToView("ForecastQuartersView").HasNoKey();
    }
}
