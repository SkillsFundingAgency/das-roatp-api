﻿using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Locations.Commands.BulkInsert;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Commands.BulkInsert
{
    [TestFixture]
    public class BulkInsertProviderLocationsCommandValidatorTests
    {
        private readonly string _userId = "userid";
        [Test]
        public async Task ValidateUkprn_InValid_ReturnsError()
        {
            var command = new BulkInsertProviderLocationsCommand
            {
                Ukprn = 10012002,
                LarsCode = 123,
                UserId = _userId
            };

            var sut = new BulkInsertProviderLocationsCommandValidator(Mock.Of<IRegionReadRepository>(), Mock.Of<IProviderReadRepository>(), Mock.Of<IProviderCourseReadRepository>(), Mock.Of<IProviderLocationsReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.Ukprn);
        }

        [Test]
        public async Task ValidateLarsCode_InValid_ReturnsError()
        {
            var command = new BulkInsertProviderLocationsCommand
            {
                Ukprn = 10012002,
                LarsCode = 123,
                UserId = _userId
            };
            var sut = new BulkInsertProviderLocationsCommandValidator(Mock.Of<IRegionReadRepository>(), Mock.Of<IProviderReadRepository>(), Mock.Of<IProviderCourseReadRepository>(), Mock.Of<IProviderLocationsReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.LarsCode);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public async Task ValidateUserId_Empty_ReturnsError(string userId)
        {
            var command = new BulkInsertProviderLocationsCommand
            {
                Ukprn = 10012002,
                LarsCode = 123,
                UserId = userId
            };
            var sut = new BulkInsertProviderLocationsCommandValidator(Mock.Of<IRegionReadRepository>(), Mock.Of<IProviderReadRepository>(), Mock.Of<IProviderCourseReadRepository>(), Mock.Of<IProviderLocationsReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.UserId);
        }
    }
}
