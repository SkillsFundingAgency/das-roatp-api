using FluentValidation.TestHelper;
using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocationDetails;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Queries.ProviderLocations
{
    [TestFixture]
    public class GetProviderLocationDetailsQueryValidatorTests
    {
        [Test]
        public async Task ValidateUkprn_InValid_ReturnsError()
        {
            var query = new GetProviderLocationDetailsQuery(10012002, Guid.NewGuid());
    
            var sut = new GetProviderLocationDetailsQueryValidator(Mock.Of<IProviderReadRepository>(), Mock.Of<IProviderLocationsReadRepository>());

            var result = await sut.TestValidateAsync(query);

            result.ShouldHaveValidationErrorFor(c => c.Ukprn);
        }

        [Test]
        public async Task ValidateId_InvalidId_ReturnsError()
        {
            var query = new GetProviderLocationDetailsQuery(10012002, Guid.Empty);

            var sut = new GetProviderLocationDetailsQueryValidator(Mock.Of<IProviderReadRepository>(), Mock.Of<IProviderLocationsReadRepository>());

            var result = await sut.TestValidateAsync(query);

            result.ShouldHaveValidationErrorFor(c => c.Id);
        }

        [Test]
        public async Task Validate_InvalidUkprnLarsCode_CourseDataNotFound()
        {
            var query = new GetProviderLocationDetailsQuery(10012002, Guid.NewGuid());

            var sut = new GetProviderLocationDetailsQueryValidator(Mock.Of<IProviderReadRepository>(), Mock.Of<IProviderLocationsReadRepository>());

            var result = await sut.TestValidateAsync(query);

            result.ShouldHaveValidationErrorFor(c => c.Id);
        }
    }
}
