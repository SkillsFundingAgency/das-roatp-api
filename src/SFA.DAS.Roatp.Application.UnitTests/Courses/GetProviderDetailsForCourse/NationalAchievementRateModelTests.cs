using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetProviderDetailsForCourse
{
    [TestFixture]
    public class NationalAchievementRateModelTests
    {
        [Test, RecursiveMoqAutoData]
        public void Operator_PopulatesModelFromEntity(NationalAchievementRate nar)
        {
            var model = (NationalAchievementRateModel)nar;

            model.Should().NotBeNull();
            nar.ApprenticeshipLevel.Should().Be(model.Level);

            model.Should().BeEquivalentTo(nar, c => c
                .Excluding(s => s.Id)
                .Excluding(s => s.ApprenticeshipLevel)
                .Excluding(s => s.SectorSubjectAreaTier1)
                .Excluding(s => s.Ukprn));
        }
    }
}
