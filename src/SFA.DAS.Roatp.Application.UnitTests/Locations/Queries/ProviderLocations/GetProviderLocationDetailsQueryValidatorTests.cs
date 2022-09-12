using FluentValidation.TestHelper;
using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocationDetails;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Queries.ProviderLocations
{
    [TestFixture]
    public class GetProviderLocationDetailsQueryValidatorTests
    {
        public const string InvalidUkprnErrorMessage = "Invalid ukprn";
        public const string ProviderNotFoundErrorMessage = "No provider found with given ukprn";
        public const string InvalidIdErrorMessage = "Invalid id";
        public const string ProviderLocationNotFoundErrorMessage = "No provider location found with given ukprn and id";

        [Test]
        public async Task ValidateUkprn_InValidNumber_ReturnsError()
        {
            var query = new GetProviderLocationDetailsQuery(10000000, Guid.NewGuid());

            var sut = new GetProviderLocationDetailsQueryValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderLocationsReadRepository>());

            var result = await sut.TestValidateAsync(query);

            result.ShouldHaveValidationErrorFor(c => c.Ukprn).WithErrorMessage(InvalidUkprnErrorMessage);
        }

        [Test]
        public async Task ValidateUkprn_InValid_ReturnsError()
        {
            var query = new GetProviderLocationDetailsQuery(10012002, Guid.NewGuid());

            var repoMockProviderReadRepository = new Mock<IProvidersReadRepository>();

            repoMockProviderReadRepository.Setup(x => x.GetByUkprn(query.Ukprn)).ReturnsAsync((Provider)null);

            var sut = new GetProviderLocationDetailsQueryValidator(repoMockProviderReadRepository.Object, Mock.Of<IProviderLocationsReadRepository>());

            var result = await sut.TestValidateAsync(query);

            result.ShouldHaveValidationErrorFor(c => c.Ukprn).WithErrorMessage(ProviderNotFoundErrorMessage);
        }

        [Test]
        public async Task ValidateId_InvalidId_ReturnsError()
        {
            var query = new GetProviderLocationDetailsQuery(10012002, Guid.Empty);

            var sut = new GetProviderLocationDetailsQueryValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderLocationsReadRepository>());

            var result = await sut.TestValidateAsync(query);

            result.ShouldHaveValidationErrorFor(c => c.Id).WithErrorMessage(InvalidIdErrorMessage);
        }

        [Test]
        public async Task Validate_InvalidUkprnId_ProviderLocationNotFound()
        {
            var query = new GetProviderLocationDetailsQuery(10012002, Guid.NewGuid());

            var repoMockProviderLocationsRead = new Mock<IProviderLocationsReadRepository>();

            repoMockProviderLocationsRead.Setup(x => x.GetProviderLocation(query.Ukprn, query.Id)).ReturnsAsync((ProviderLocation) null);


            var sut = new GetProviderLocationDetailsQueryValidator(Mock.Of<IProvidersReadRepository>(), repoMockProviderLocationsRead.Object);

            var result = await sut.TestValidateAsync(query);

            result.ShouldHaveValidationErrorFor(c => c.Id).WithErrorMessage(ProviderLocationNotFoundErrorMessage);
        }

        [Test]
        public async Task Validate_ValidUkprn_ReturnsNoErrorsForUkprn()
        {
            var query = new GetProviderLocationDetailsQuery(10012002, Guid.NewGuid());

            var repoMockProviderReadRepository = new Mock<IProvidersReadRepository>();

            repoMockProviderReadRepository.Setup(x => x.GetByUkprn(query.Ukprn)).ReturnsAsync(new Provider());

            var sut = new GetProviderLocationDetailsQueryValidator(repoMockProviderReadRepository.Object, Mock.Of<IProviderLocationsReadRepository>());

            var result = await sut.TestValidateAsync(query);

            result.ShouldNotHaveValidationErrorFor(c => c.Ukprn);
        }

        [Test]
        public async Task Validate_validId_ReturnsNoErrorsForId()
        {
            var query = new GetProviderLocationDetailsQuery(10012002, Guid.NewGuid());

            var repoMockProviderLocationsRead = new Mock<IProviderLocationsReadRepository>();

            repoMockProviderLocationsRead.Setup(x => x.GetProviderLocation(query.Ukprn, query.Id)).ReturnsAsync(new ProviderLocation());

            var sut = new GetProviderLocationDetailsQueryValidator(Mock.Of<IProvidersReadRepository>(), repoMockProviderLocationsRead.Object);

            var result = await sut.TestValidateAsync(query);

            result.ShouldNotHaveValidationErrorFor(c => c.Id);
        }
    }
}
