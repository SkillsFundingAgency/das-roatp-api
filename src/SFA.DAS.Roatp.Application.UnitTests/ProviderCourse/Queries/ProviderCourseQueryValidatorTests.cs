using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Queries;

[TestFixture]
public class ProviderCourseQueryValidatorTests
{
    private Mock<IProviderCourseTypesReadRepository> _providerCourseTypesReadRepositoryMock;
    private Mock<IStandardsReadRepository> _standardsReadRepositoryMock;

    [SetUp]
    public void BeforeEach()
    {
        _providerCourseTypesReadRepositoryMock = new Mock<IProviderCourseTypesReadRepository>();
        _standardsReadRepositoryMock = new Mock<IStandardsReadRepository>();
        _standardsReadRepositoryMock.Setup(x => x.GetStandard(It.IsAny<string>()))
            .ReturnsAsync(new Standard { CourseType = CourseType.Apprenticeship });
        _providerCourseTypesReadRepositoryMock.Setup(x => x.GetProviderCourseTypesByUkprn(It.IsAny<int>()))
            .ReturnsAsync(new List<ProviderCourseType> { new ProviderCourseType { CourseType = CourseType.Apprenticeship } });
    }

    [TestCase(10000001, true)]
    [TestCase(10000000, false)]
    [TestCase(100000000, false)]
    public async Task Validate_AcceptsEightDigitNumbersOnly(int ukprn, bool expectedResult)
    {
        var larsCode = "1";
        var query = new GetProviderCourseQuery(ukprn, larsCode);
        var repoMock = new Mock<IProvidersReadRepository>();
        var repoMockProvideCourse = new Mock<IProviderCoursesReadRepository>();
        repoMock.Setup(x => x.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());
        repoMockProvideCourse.Setup(x => x.GetProviderCourse(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(new Domain.Entities.ProviderCourse());
        var sut = new GetProviderCourseQueryValidator(repoMock.Object, repoMockProvideCourse.Object, _standardsReadRepositoryMock.Object, _providerCourseTypesReadRepositoryMock.Object);

        var result = await sut.ValidateAsync(query);

        result.IsValid.Should().Be(expectedResult);
    }

    [Test]
    public async Task Validate_InvalidUkprnLarsCode()
    {
        int ukprn = 1;
        string larsCode = "";
        int expectedTimesRepoIsInvoked = 0;
        string expectedErrorMessage1 = UkprnValidator.InvalidUkprnErrorMessage;
        string expectedErrorMessage2 = LarsCodeUkprnCombinationValidator.InvalidLarsCodeErrorMessage;
        var query = new GetProviderCourseQuery(ukprn, larsCode);
        var repoMockProvideCourse = new Mock<IProviderCoursesReadRepository>();
        var repoMock = new Mock<IProvidersReadRepository>();
        var sut = new GetProviderCourseQueryValidator(repoMock.Object, repoMockProvideCourse.Object, _standardsReadRepositoryMock.Object, _providerCourseTypesReadRepositoryMock.Object);

        var result = await sut.ValidateAsync(query);

        repoMock.Verify(x => x.GetByUkprn(It.IsAny<int>()), Times.Exactly(expectedTimesRepoIsInvoked));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
        expectedErrorMessage1.Should().Be(result.Errors[0].ErrorMessage);
        expectedErrorMessage2.Should().Be(result.Errors[1].ErrorMessage);
    }

    [Test]
    public async Task Validate_InvalidUkprnLarsCode_CourseDataNotFound()
    {
        int ukprn = 10012002;
        var larsCode = "1";
        int expectedTimesRepoIsInvoked = 2;
        string expectedErrorMessage1 = LarsCodeUkprnCombinationValidator.ProviderCourseNotFoundErrorMessage;
        var query = new GetProviderCourseQuery(ukprn, larsCode);
        var repoMockProvideCourse = new Mock<IProviderCoursesReadRepository>();
        var repoMock = new Mock<IProvidersReadRepository>();
        repoMock.Setup(r => r.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());
        repoMockProvideCourse.Setup(x => x.GetProviderCourse(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync((Domain.Entities.ProviderCourse)null);
        var sut = new GetProviderCourseQueryValidator(repoMock.Object, repoMockProvideCourse.Object, _standardsReadRepositoryMock.Object, _providerCourseTypesReadRepositoryMock.Object);

        var result = await sut.ValidateAsync(query);

        repoMock.Verify(x => x.GetByUkprn(It.IsAny<int>()), Times.Exactly(expectedTimesRepoIsInvoked));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        expectedErrorMessage1.Should().Be(result.Errors[0].ErrorMessage);
    }

    [Test]
    public async Task Validate_ProviderDoesNotSupportCourseType_ShouldBeInvalid()
    {
        // Arrange - standard is Apprenticeship but provider only offers ApprenticeshipUnit
        _standardsReadRepositoryMock.Setup(x => x.GetStandard(It.IsAny<string>()))
            .ReturnsAsync(new Standard { CourseType = CourseType.Apprenticeship });

        _providerCourseTypesReadRepositoryMock.Setup(x => x.GetProviderCourseTypesByUkprn(It.IsAny<int>()))
            .ReturnsAsync(new List<ProviderCourseType> { new ProviderCourseType { CourseType = CourseType.ShortCourse } });

        var ukprn = 10000001;
        var larsCode = "1";
        var query = new GetProviderCourseQuery(ukprn, larsCode);

        var providersRepo = new Mock<IProvidersReadRepository>();
        providersRepo.Setup(x => x.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());

        var providerCoursesRepo = new Mock<IProviderCoursesReadRepository>();
        providerCoursesRepo.Setup(x => x.GetProviderCourse(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(new Domain.Entities.ProviderCourse());

        var sut = new GetProviderCourseQueryValidator(providersRepo.Object, providerCoursesRepo.Object, _standardsReadRepositoryMock.Object, _providerCourseTypesReadRepositoryMock.Object);

        // Act
        var result = await sut.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        CourseTypeValidator.ProviderCourseTypeNotFoundErrorMessage.Should().Be(result.Errors[0].ErrorMessage);
    }

    [Test]
    public async Task Validate_StandardNotFound_ShouldBeInvalid()
    {
        // Arrange - standard lookup returns null
        _standardsReadRepositoryMock.Setup(x => x.GetStandard(It.IsAny<string>()))
            .ReturnsAsync((Standard)null);

        _providerCourseTypesReadRepositoryMock.Setup(x => x.GetProviderCourseTypesByUkprn(It.IsAny<int>()))
            .ReturnsAsync(new List<ProviderCourseType> { new ProviderCourseType { CourseType = CourseType.Apprenticeship } });

        var ukprn = 10000001;
        var larsCode = "1";
        var query = new GetProviderCourseQuery(ukprn, larsCode);

        var providersRepo = new Mock<IProvidersReadRepository>();
        providersRepo.Setup(x => x.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());

        var providerCoursesRepo = new Mock<IProviderCoursesReadRepository>();
        providerCoursesRepo.Setup(x => x.GetProviderCourse(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(new Domain.Entities.ProviderCourse());

        var sut = new GetProviderCourseQueryValidator(providersRepo.Object, providerCoursesRepo.Object, _standardsReadRepositoryMock.Object, _providerCourseTypesReadRepositoryMock.Object);

        // Act
        var result = await sut.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        CourseTypeValidator.ProviderCourseTypeNotFoundErrorMessage.Should().Be(result.Errors[0].ErrorMessage);
    }

    [Test]
    public async Task Validate_ProviderCourseTypesRepoReturnsNull_ShouldBeInvalid()
    {
        // Arrange - provider course types repo returns null
        _providerCourseTypesReadRepositoryMock.Setup(x => x.GetProviderCourseTypesByUkprn(It.IsAny<int>()))
            .ReturnsAsync((List<ProviderCourseType>)null);

        _standardsReadRepositoryMock.Setup(x => x.GetStandard(It.IsAny<string>()))
            .ReturnsAsync(new Standard { CourseType = CourseType.Apprenticeship });

        var ukprn = 10000001;
        var larsCode = "1";
        var query = new GetProviderCourseQuery(ukprn, larsCode);

        var providersRepo = new Mock<IProvidersReadRepository>();
        providersRepo.Setup(x => x.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());

        var providerCoursesRepo = new Mock<IProviderCoursesReadRepository>();
        providerCoursesRepo.Setup(x => x.GetProviderCourse(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(new Domain.Entities.ProviderCourse());

        var sut = new GetProviderCourseQueryValidator(providersRepo.Object, providerCoursesRepo.Object, _standardsReadRepositoryMock.Object, _providerCourseTypesReadRepositoryMock.Object);

        // Act
        var result = await sut.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        CourseTypeValidator.ProviderCourseTypeNotFoundErrorMessage.Should().Be(result.Errors[0].ErrorMessage);
    }

    [Test]
    public async Task Validate_CourseTypeMatchesProvider_ShouldBeValid()
    {
        // Arrange - standard and provider course types both Apprenticeship (default SetUp)
        var ukprn = 10000001;
        var larsCode = "1";
        var query = new GetProviderCourseQuery(ukprn, larsCode);

        var providersRepo = new Mock<IProvidersReadRepository>();
        providersRepo.Setup(x => x.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());

        var providerCoursesRepo = new Mock<IProviderCoursesReadRepository>();
        providerCoursesRepo.Setup(x => x.GetProviderCourse(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(new Domain.Entities.ProviderCourse());

        var sut = new GetProviderCourseQueryValidator(providersRepo.Object, providerCoursesRepo.Object, _standardsReadRepositoryMock.Object, _providerCourseTypesReadRepositoryMock.Object);

        // Act
        var result = await sut.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}