using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Data.Configuration;

[ExcludeFromCodeCoverage]
public class ProviderCourseLocationDetailsModelConfiguration : IEntityTypeConfiguration<ProviderCourseLocationDetailsModel>
{
    public void Configure(EntityTypeBuilder<ProviderCourseLocationDetailsModel> builder)
    {
        builder.HasNoKey();
    }
}