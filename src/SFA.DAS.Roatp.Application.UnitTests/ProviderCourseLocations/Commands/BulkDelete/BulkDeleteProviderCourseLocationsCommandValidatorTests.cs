﻿using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkDelete;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseLocations.Commands.BulkDelete
{
    [TestFixture]
    public class BulkDeleteProviderCourseLocationsCommandValidatorTests
    {
        [Test]
        public async Task ValidateDeleteOption_NoneSelected_ReturnsError()
        {
            var command = new BulkDeleteProviderCourseLocationsCommand(10012002, 123, DeleteOptions.None);

            var sut = new BulkDeleteProviderCourseLocationsCommandValidator(Mock.Of<IProviderReadRepository>(), Mock.Of<IProviderCourseReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.DeleteOptions);
        }

        [TestCase(DeleteOptions.DeleteProviderLocations)]
        [TestCase(DeleteOptions.DeleteEmployerLocations)]
        public async Task ValidateDeleteOption_Selected_ReturnsValid(DeleteOptions options)
        {
            var command = new BulkDeleteProviderCourseLocationsCommand(10012002, 123, options);

            var sut = new BulkDeleteProviderCourseLocationsCommandValidator(Mock.Of<IProviderReadRepository>(), Mock.Of<IProviderCourseReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldNotHaveValidationErrorFor(c => c.DeleteOptions);
        }

        [Test]
        public async Task ValidateUkprn_InValid_ReturnsError()
        {
            var command = new BulkDeleteProviderCourseLocationsCommand(10012002, 123, DeleteOptions.DeleteProviderLocations);

            var sut = new BulkDeleteProviderCourseLocationsCommandValidator(Mock.Of<IProviderReadRepository>(), Mock.Of<IProviderCourseReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.Ukprn);
        }

        [Test]
        public async Task ValidateLarsCode_InValid_ReturnsError()
        {
            var command = new BulkDeleteProviderCourseLocationsCommand(10012002, 123, DeleteOptions.DeleteProviderLocations);

            var sut = new BulkDeleteProviderCourseLocationsCommandValidator(Mock.Of<IProviderReadRepository>(), Mock.Of<IProviderCourseReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.LarsCode);
        }

    }
}
