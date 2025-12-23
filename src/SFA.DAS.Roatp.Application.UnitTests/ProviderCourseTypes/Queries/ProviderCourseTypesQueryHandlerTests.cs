using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseTypes.Queries.GetProviderCourseTypes;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseTypes.Queries
{
    [TestFixture]
    public class ProviderCourseTypesQueryHandlerTests
    {
        [Test, RecursiveMoqAutoData()]
        public async Task Handle_ReturnsResult(
            ProviderCourseType providerCourseType,
            List<ProviderCourseType> providerCourseTypes,
            [Frozen] Mock<IProviderCourseTypesReadRepository> repo,
            GetProviderCourseTypesQuery query,
            GetProviderCourseTypesQueryHandler sut,
            CancellationToken cancellationToken)
        {
            repo.Setup(r => r.GetProviderCourseTypesByUkprn(It.IsAny<int>())).ReturnsAsync(providerCourseTypes);

            var response = await sut.Handle(query, cancellationToken);

            response.Should().NotBeNull();
            response.Result.Count.Should().Be(providerCourseTypes.Count);

            response.Result[0].CourseType.Should().Be(providerCourseTypes[0].CourseType);
        }

        [Test, RecursiveMoqAutoData()]
        public async Task Handle_NoData_ReturnsEmptyList(
            ProviderCourseType providerCourseType,
            [Frozen] Mock<IProviderCourseTypesReadRepository> repo,
            GetProviderCourseTypesQuery query,
            GetProviderCourseTypesQueryHandler sut,
            CancellationToken cancellationToken)
        {
            List<ProviderCourseType> providerCourseTypes = new List<ProviderCourseType>();
            repo.Setup(r => r.GetProviderCourseTypesByUkprn(It.IsAny<int>())).ReturnsAsync(providerCourseTypes);

            var response = await sut.Handle(query, cancellationToken);

            response.Should().NotBeNull();
            response.Result.Count.Should().Be(0);
        }
    }
}
