using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProvider;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Queries
{
    [TestFixture]
    public class GetProviderQueryResultTests
    {
        [Test, RecursiveMoqAutoData]
        public void Operator_PopulatesModelFromEntity(Provider provider)
        {
            var model = (GetProviderQueryResult)provider;

            model.Should().BeEquivalentTo(provider, c => c
                .Excluding(s => s.Id)
                .Excluding(s => s.Locations)
                .Excluding(s => s.Courses)
                .Excluding(s=>s.EmployerSatisfaction)
                .Excluding(s=>s.LearnerSatisfaction)
                .Excluding(s => s.NationalAchievementRates)
                .Excluding(s => s.Address)
            );
        }
    }
}
