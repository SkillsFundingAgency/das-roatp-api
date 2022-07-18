using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.AddNationalLocation;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseLocations.Commands.AddNationalLocation
{
    [TestFixture]
    public class AddNationalLocationToProviderCourseLocationsCommandValidatorTests
    {
        private Mock<IProviderReadRepository> _providerReadRepositoryMock;
        private Mock<IProviderCourseReadRepository> _providerCourseReadRepositoryMock;
        private AddNationalLocationToProviderCourseLocationsCommand _command;

        [SetUp]
        public void Before_Each_Test()
        {
            _providerReadRepositoryMock = new Mock<IProviderReadRepository>();
            _providerReadRepositoryMock
                .Setup(x => x.GetByUkprn(It.IsAny<int>()))
                .ReturnsAsync(new Provider());

            _providerCourseReadRepositoryMock = new Mock<IProviderCourseReadRepository>();
            _providerCourseReadRepositoryMock
                .Setup(x => x.GetProviderCourse(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new Domain.Entities.ProviderCourse());

            _command = new AddNationalLocationToProviderCourseLocationsCommand(10012002, 123, "user");
        }

        [Test, RecursiveMoqAutoData]
        public async Task Validate_NationalAddressExists_ReturnsInvalid(List<ProviderCourseLocation> providerCourseLocations)
        {
            providerCourseLocations.First().Location.LocationType = LocationType.National;
            var providerCourseLocationRepoMock = new Mock<IProviderCourseLocationReadRepository>();
            providerCourseLocationRepoMock.Setup(r => r.GetAllProviderCourseLocations(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(providerCourseLocations);
            var sut = new AddNationalLocationToProviderCourseLocationsCommandValidator(_providerReadRepositoryMock.Object, _providerCourseReadRepositoryMock.Object, providerCourseLocationRepoMock.Object);

            var result = await sut.TestValidateAsync(_command);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
        }

        [Test, RecursiveMoqAutoData]
        public async Task Validate_NationalAddressNotExists_ReturnsValid(List<ProviderCourseLocation> providerCourseLocations)
        {
            providerCourseLocations.ForEach(l => l.Location.LocationType = LocationType.Provider);
            var providerCourseLocationRepoMock = new Mock<IProviderCourseLocationReadRepository>();
            providerCourseLocationRepoMock.Setup(r => r.GetAllProviderCourseLocations(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(providerCourseLocations);
            var sut = new AddNationalLocationToProviderCourseLocationsCommandValidator(_providerReadRepositoryMock.Object, _providerCourseReadRepositoryMock.Object, providerCourseLocationRepoMock.Object);

            var result = await sut.ValidateAsync(_command);

            result.IsValid.Should().BeTrue();
        }
    }
}
