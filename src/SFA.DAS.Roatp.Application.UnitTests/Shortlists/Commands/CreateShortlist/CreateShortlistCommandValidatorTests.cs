using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Shortlists.Commands.CreateShortlist;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Shortlists.Commands.CreateShortlist;

public class CreateShortlistCommandValidatorTests
{
    [Test, MoqAutoData]
    public async Task Validate_UserIdEmpty_Invalid(CreateShortlistCommandValidator sut)
    {
        var userId = Guid.Empty;
        CreateShortlistCommand command = new() { UserId = userId };
        var result = await sut.TestValidateAsync(command, options => options.IncludeProperties(nameof(CreateShortlistCommand.UserId)));

        result.ShouldHaveValidationErrorFor(s => s.UserId);
    }

    [Test, MoqAutoData]
    public async Task Validate_UserExceededShortlist_Invalid(
        [Frozen] Mock<IShortlistsRepository> repoMock,
        CreateShortlistCommandValidator sut,
        CreateShortlistCommand command)
    {
        repoMock.Setup(r => r.GetShortlistCount(command.UserId, It.IsAny<CancellationToken>())).ReturnsAsync(CreateShortlistCommandValidator.MaximumShortlistAllowance + 1);

        var result = await sut.TestValidateAsync(command, options => options.IncludeProperties(nameof(CreateShortlistCommand.UserId)));

        repoMock.Verify(r => r.GetShortlistCount(command.UserId, It.IsAny<CancellationToken>()), Times.Once);
        result.ShouldHaveValidationErrorFor(s => s.UserId);
    }

    [Test, MoqAutoData]
    public async Task Validate_UserDoesNotExceedsShortlistCount_Valid(
        [Frozen] Mock<IShortlistsRepository> repoMock,
        CreateShortlistCommandValidator sut,
        CreateShortlistCommand command)
    {
        repoMock.Setup(r => r.GetShortlistCount(command.UserId, It.IsAny<CancellationToken>())).ReturnsAsync(CreateShortlistCommandValidator.MaximumShortlistAllowance);

        var result = await sut.TestValidateAsync(command, options => options.IncludeProperties(nameof(CreateShortlistCommand.UserId)));

        repoMock.Verify(r => r.GetShortlistCount(command.UserId, It.IsAny<CancellationToken>()), Times.Once);
        result.ShouldNotHaveValidationErrorFor(s => s.UserId);
    }

    [Test, MoqAutoData]
    public async Task Validate_LarsCodeEmpty_Invalid(
        CreateShortlistCommandValidator sut,
        CreateShortlistCommand command)
    {
        command.LarsCode = "";

        var result = await sut.TestValidateAsync(command, options => options.IncludeProperties(nameof(CreateShortlistCommand.LarsCode)));

        result.ShouldHaveValidationErrorFor(s => s.LarsCode);
    }

    [Test, MoqAutoData]
    public async Task Validate_LarsCodeNotFound_Invalid(
        [Frozen] Mock<IStandardsReadRepository> repoMock,
        CreateShortlistCommandValidator sut,
        CreateShortlistCommand command)
    {
        command.LarsCode = "1";
        repoMock.Setup(r => r.GetStandard(command.LarsCode)).ReturnsAsync(() => null);

        var result = await sut.TestValidateAsync(command, options => options.IncludeProperties(nameof(CreateShortlistCommand.LarsCode)));

        repoMock.Verify(r => r.GetStandard(command.LarsCode), Times.Once);
        result.ShouldHaveValidationErrorFor(s => s.LarsCode);
    }

    [Test, MoqAutoData]
    public async Task Validate_LarsCodeFound_Valid(
        [Frozen] Mock<IStandardsReadRepository> repoMock,
        CreateShortlistCommandValidator sut,
        CreateShortlistCommand command)
    {
        var standard = new Standard { LarsCode = "1", Title = "standard 1" };
        command.LarsCode = "1";
        repoMock.Setup(r => r.GetStandard(command.LarsCode)).ReturnsAsync(standard);

        var result = await sut.TestValidateAsync(command, options => options.IncludeProperties(nameof(CreateShortlistCommand.LarsCode)));

        repoMock.Verify(r => r.GetStandard(command.LarsCode), Times.Once);
        result.ShouldNotHaveValidationErrorFor(s => s.LarsCode);
    }

    [Test, MoqAutoData]
    public async Task Validate_UkprnNotFound_Invalid(
    [Frozen] Mock<IProvidersReadRepository> repoMock,
    CreateShortlistCommandValidator sut,
    CreateShortlistCommand command)
    {
        command.Ukprn = 10012001;
        repoMock.Setup(r => r.GetByUkprn(command.Ukprn)).ReturnsAsync(() => null);

        var result = await sut.TestValidateAsync(command, options => options.IncludeProperties(nameof(CreateShortlistCommand.Ukprn)));

        repoMock.Verify(r => r.GetByUkprn(command.Ukprn), Times.Once);
        result.ShouldHaveValidationErrorFor(s => s.Ukprn);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Validate_UkprnFoundNoCourse_Invalid(
        [Frozen] Mock<IProvidersReadRepository> providerRepoMock,
        [Frozen] Mock<IProviderRegistrationDetailsReadRepository> prvRegRepoMock,
        CreateShortlistCommandValidator sut,
        CreateShortlistCommand command,
        Provider provider)
    {
        command.Ukprn = 10012001;
        providerRepoMock.Setup(r => r.GetByUkprn(command.Ukprn)).ReturnsAsync(provider);
        prvRegRepoMock.Setup(r => r.IsMainActiveProvider(command.Ukprn, command.LarsCode)).ReturnsAsync(false);

        var result = await sut.TestValidateAsync(command, options => options.IncludeProperties(nameof(CreateShortlistCommand.Ukprn)));

        providerRepoMock.Verify(r => r.GetByUkprn(command.Ukprn), Times.Once);
        prvRegRepoMock.Verify(r => r.IsMainActiveProvider(command.Ukprn, command.LarsCode), Times.Once);
        result.ShouldHaveValidationErrorFor(s => s.Ukprn);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Validate_UkprnFoundWithCourse_Valid(
        [Frozen] Mock<IProvidersReadRepository> providerRepoMock,
        [Frozen] Mock<IProviderRegistrationDetailsReadRepository> prvRegRepoMock,
        CreateShortlistCommandValidator sut,
        CreateShortlistCommand command,
        Provider provider)
    {
        command.Ukprn = 10012001;
        providerRepoMock.Setup(r => r.GetByUkprn(command.Ukprn)).ReturnsAsync(provider);
        prvRegRepoMock.Setup(r => r.IsMainActiveProvider(command.Ukprn, command.LarsCode)).ReturnsAsync(true);

        var result = await sut.TestValidateAsync(command, options => options.IncludeProperties(nameof(CreateShortlistCommand.Ukprn)));

        providerRepoMock.Verify(r => r.GetByUkprn(command.Ukprn), Times.Once);
        prvRegRepoMock.Verify(r => r.IsMainActiveProvider(command.Ukprn, command.LarsCode), Times.Once);
        result.ShouldNotHaveValidationErrorFor(s => s.Ukprn);
    }

    [Test, MoqAutoData]
    public async Task Validate_LocationNotGiven_EmptyLatLon_Valid(
        CreateShortlistCommandValidator sut,
        CreateShortlistCommand command)
    {
        command.LocationDescription = null;
        command.Latitude = null;
        command.Longitude = null;

        var result = await sut.TestValidateAsync(command, options => options.IncludeProperties(nameof(CreateShortlistCommand.Latitude), nameof(CreateShortlistCommand.Longitude)));

        result.ShouldNotHaveValidationErrorFor(s => s.Latitude);
        result.ShouldNotHaveValidationErrorFor(s => s.Longitude);
    }

    [Test, MoqAutoData]
    public async Task Validate_LocationGiven_EmptyLatLon_Invalid(
     CreateShortlistCommandValidator sut,
     CreateShortlistCommand command)
    {
        command.LocationDescription = "MK4 4ET";
        command.Latitude = null;
        command.Longitude = null;
        sut.ClassLevelCascadeMode = FluentValidation.CascadeMode.Continue;

        var result = await sut.TestValidateAsync(command, options => options.IncludeProperties(nameof(CreateShortlistCommand.Latitude), nameof(CreateShortlistCommand.Longitude)));

        result.ShouldHaveValidationErrorFor(s => s.Latitude);
        result.ShouldHaveValidationErrorFor(s => s.Longitude);
    }

    [Test, MoqAutoData]
    public async Task Validate_LocationGiven_LatOutOfRange_Invalid(
     CreateShortlistCommandValidator sut,
     CreateShortlistCommand command)
    {
        command.LocationDescription = "MK4 4ET";
        command.Latitude = 91;

        var result = await sut.TestValidateAsync(command, options => options.IncludeProperties(nameof(CreateShortlistCommand.Latitude)));

        result.ShouldHaveValidationErrorFor(s => s.Latitude);
    }

    [Test, MoqAutoData]
    public async Task Validate_LocationGiven_LonOutOfRange_Invalid(
     CreateShortlistCommandValidator sut,
     CreateShortlistCommand command)
    {
        command.LocationDescription = "MK4 4ET";
        command.Longitude = -181;

        var result = await sut.TestValidateAsync(command, options => options.IncludeProperties(nameof(CreateShortlistCommand.Longitude)));

        result.ShouldHaveValidationErrorFor(s => s.Longitude);
    }

    [Test, MoqAutoData]
    public async Task Validate_LocationGiven_LatLonInRange_Valid(
     CreateShortlistCommandValidator sut,
     CreateShortlistCommand command)
    {
        command.LocationDescription = "MK4 4ET";
        command.Latitude = 90;
        command.Longitude = -180;

        var result = await sut.TestValidateAsync(command, options => options.IncludeProperties(nameof(CreateShortlistCommand.Longitude), nameof(CreateShortlistCommand.Latitude)));

        result.ShouldNotHaveValidationErrorFor(s => s.Latitude);
        result.ShouldNotHaveValidationErrorFor(s => s.Longitude);
    }
}
