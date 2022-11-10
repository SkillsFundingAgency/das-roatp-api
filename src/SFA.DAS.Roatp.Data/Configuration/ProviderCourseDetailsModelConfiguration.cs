using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Data.Configuration
{
    [ExcludeFromCodeCoverage]
    public class ProviderCourseDetailsModelConfiguration: IEntityTypeConfiguration<ProviderCourseDetailsModel>
    {
        public void Configure(EntityTypeBuilder<ProviderCourseDetailsModel> builder)
        {
            builder.HasNoKey();
        }
    }
}
