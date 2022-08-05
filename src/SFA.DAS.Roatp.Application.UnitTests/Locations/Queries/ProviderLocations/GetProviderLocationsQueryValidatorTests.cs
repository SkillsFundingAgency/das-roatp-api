using FluentValidation.TestHelper;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocations;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Queries.ProviderLocations
{
    [TestFixture]
    public class GetProviderLocationsQueryValidatorTests
    {
        [Test]
        public async Task ValidateUkprn_InValid_ReturnsError()
        {
            var query = new GetProviderLocationsQuery(10012002);
    
            var sut = new GetProviderLocationsQueryValidator(Mock.Of<IProviderReadRepository>());

            var result = await sut.TestValidateAsync(query);

            result.ShouldHaveValidationErrorFor(c => c.Ukprn);
        }
    }
}
