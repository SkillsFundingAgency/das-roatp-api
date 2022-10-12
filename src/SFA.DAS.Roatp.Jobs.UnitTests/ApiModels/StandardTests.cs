using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Jobs.ApiModels.Lookup;

namespace SFA.DAS.Roatp.Jobs.UnitTests.ApiModels
{
    [TestFixture]
    public class StandardTests
    {
        [Test]
        [AutoData]
        public void Operator_ReturnsDomainEntity(Standard source)
        {
            var expectedLevel = 1;
            source.Level = expectedLevel.ToString();
            var target = (Domain.Entities.Standard) source;

            target.Should().BeEquivalentTo(source, o => 
            {
                o.Excluding(s => s.Level);
                o.Excluding(s => s.ApprovalBody);
                o.WithMapping<Domain.Entities.Standard>(s => s.SectorSubjectAreaTier2Description, x => x.SectorSubjectArea);
                return o;
            });
            target.Level.Should().Be(expectedLevel);
        }

        [TestCase("")]
        [TestCase("  ")]
        [TestCase(null)]
        public void Operator_ApprovalBodyIsNullOrEmpty_ReturnsDomainEntityWithApprovalBodySetToNull(string approvalBody)
        {
            var fixture = new Fixture();
            var source = fixture.Build<Standard>()
                .With(s => s.Level, "1")
                .With(s => s.ApprovalBody, approvalBody)
                .Create();
            var target = (Domain.Entities.Standard)source;

            target.Should().BeEquivalentTo(source, o =>
            {
                o.Excluding(s => s.Level);
                o.Excluding(s => s.ApprovalBody);
                o.WithMapping<Domain.Entities.Standard>(s => s.SectorSubjectAreaTier2Description, x => x.SectorSubjectArea);
                return o;
            });
            target.ApprovalBody.Should().BeNull();
        }
    }
}
