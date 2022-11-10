using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.AddNationalLocation;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseLocations.Commands.AddNationalLocation
{
    [TestFixture]
    public class AddNationalLocationToProviderCourseLocationsCommandValidatorTests
    {
        private Mock<IProvidersReadRepository> _providersReadRepositoryMock;
        private Mock<IProviderCoursesReadRepository> _providerCoursesReadRepositoryMock;
        private AddNationalLocationToProviderCourseLocationsCommand _command;

        [SetUp]
        public void Before_Each_Test()
        {
            _providersReadRepositoryMock = new Mock<IProvidersReadRepository>();
            _providersReadRepositoryMock
                .Setup(x => x.GetByUkprn(It.IsAny<int>()))
                .ReturnsAsync(new Provider());

            _providerCoursesReadRepositoryMock = new Mock<IProviderCoursesReadRepository>();
            _providerCoursesReadRepositoryMock
                .Setup(x => x.GetProviderCourse(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new Domain.Entities.ProviderCourse());

            _command = new AddNationalLocationToProviderCourseLocationsCommand(10012002, 123, "user");
        }

        [Test, RecursiveMoqAutoData]
        public async Task Validate_NationalAddressExists_ReturnsInvalid(List<ProviderCourseLocation> providerCourseLocations)
        {
            providerCourseLocations.First().Location.LocationType = LocationType.National;
            var providerCourseLocationsRepoMock = new Mock<IProviderCourseLocationsReadRepository>();
            providerCourseLocationsRepoMock.Setup(r => r.GetAllProviderCourseLocations(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(providerCourseLocations);
            var sut = new AddNationalLocationToProviderCourseLocationsCommandValidator(_providersReadRepositoryMock.Object, _providerCoursesReadRepositoryMock.Object, providerCourseLocationsRepoMock.Object);

            var result = await sut.TestValidateAsync(_command);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
        }

        [Test, RecursiveMoqAutoData]
        public async Task Validate_NationalAddressNotExists_ReturnsValid(List<ProviderCourseLocation> providerCourseLocations)
        {
            providerCourseLocations.ForEach(l => l.Location.LocationType = LocationType.Provider);
            var providerCourseLocationsRepoMock = new Mock<IProviderCourseLocationsReadRepository>();
            providerCourseLocationsRepoMock.Setup(r => r.GetAllProviderCourseLocations(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(providerCourseLocations);
            var sut = new AddNationalLocationToProviderCourseLocationsCommandValidator(_providersReadRepositoryMock.Object, _providerCoursesReadRepositoryMock.Object, providerCourseLocationsRepoMock.Object);

            var result = await sut.ValidateAsync(_command);

            result.IsValid.Should().BeTrue();
        }
    }
}
