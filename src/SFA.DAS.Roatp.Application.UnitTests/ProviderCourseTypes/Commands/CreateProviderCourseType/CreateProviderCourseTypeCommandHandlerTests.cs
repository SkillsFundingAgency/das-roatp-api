using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseTypes.Commands.CreateProviderCourseType;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseTypes.Commands.CreateProviderCourseType;

public class CreateProviderCourseTypeCommandHandlerTests
{
    [Test, MoqAutoData]
    public async Task WhenHandlingCommandWithApprenticeshipCourseType_ThenRepositoryIsInvokedWithApprenticeshipCourseType(
    [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
    [Greedy] CreateProviderCourseTypeCommandHandler sut)
    {
        // Arrange
        int ukprn = 12345678;

        var request = new AddCourseTypesModel
        {
            CourseTypes = ["Apprenticeship"],
            UserId = "TestUserId",
            UserDisplayName = "Test User"
        };

        var command = new CreateProviderCourseTypeCommand(ukprn, request);

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        providerCourseTypesRepository.Verify(x => x.CreateProviderCourseType(
            It.Is<IEnumerable<ProviderCourseType>>(courseTypes =>
                courseTypes.Count() == 1 &&
                courseTypes.Any(c =>
                    c.Ukprn == command.Ukprn &&
                    c.CourseType == CourseType.Apprenticeship)),
            command.UserId,
            command.UserDisplayName,
            command.Ukprn,
            AuditEventTypes.CreateProviderCourseType,
            CancellationToken.None),
            Times.Once);
    }

    [Test, MoqAutoData]
    public async Task WhenHandlingCommandWithShortCourseCourseType_ThenVerifyRepositoryIsInvokedCorrectly(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Greedy] CreateProviderCourseTypeCommandHandler sut)
    {
        // Arrange
        int ukprn = 12345678;

        var request = new AddCourseTypesModel
        {
            CourseTypes = ["Apprenticeship"],
            UserId = "TestUserId",
            UserDisplayName = "Test User"
        };

        var command = new CreateProviderCourseTypeCommand(ukprn, request);

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        providerCourseTypesRepository.Verify(x => x.CreateProviderCourseType(
            It.Is<IEnumerable<ProviderCourseType>>(courseTypes =>
                courseTypes.Count() == 1 &&
                courseTypes.Any(c =>
                    c.Ukprn == command.Ukprn &&
                    c.CourseType == CourseType.ShortCourse)),
            command.UserId,
            command.UserDisplayName,
            command.Ukprn,
            AuditEventTypes.CreateProviderCourseType,
            CancellationToken.None),
            Times.Once);
    }

    [Test, MoqAutoData]
    public async Task WhenHandlingCommandWithBothCourseTypes_ThenVerifyRepositoryIsInvokedCorrectly(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepository,
        [Greedy] CreateProviderCourseTypeCommandHandler sut)
    {
        // Arrange
        int ukprn = 12345678;

        var request = new AddCourseTypesModel
        {
            CourseTypes = ["Apprenticeship"],
            UserId = "TestUserId",
            UserDisplayName = "Test User"
        };

        var command = new CreateProviderCourseTypeCommand(ukprn, request);

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        providerCourseTypesRepository.Verify(x => x.CreateProviderCourseType(
            It.Is<IEnumerable<ProviderCourseType>>(courseTypes =>
                courseTypes.Count() == 2 &&
                courseTypes.Any(c =>
                    c.Ukprn == command.Ukprn &&
                    c.CourseType == CourseType.Apprenticeship) &&
                courseTypes.Any(c =>
                    c.Ukprn == command.Ukprn &&
                    c.CourseType == CourseType.ShortCourse)),
            command.UserId,
            command.UserDisplayName,
            command.Ukprn,
            AuditEventTypes.CreateProviderCourseType,
            CancellationToken.None),
            Times.Once);
    }
}
