using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Data.Configuration;

[ExcludeFromCodeCoverage]
public class ProviderCourseDetailsSummaryModelConfiguration : IEntityTypeConfiguration<ProviderCourseDetailsSummaryModel>
{
    public void Configure(EntityTypeBuilder<ProviderCourseDetailsSummaryModel> builder)
    {
        builder.HasNoKey();
    }
}