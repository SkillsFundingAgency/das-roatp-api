using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProvider;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProviderStatus;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Queries.GetProviderStatus
{
    [TestFixture]
    public class GetProviderStatusQueryValidatorTests
    {
        [TestCase(10000001, true)]
        [TestCase(10000000, false)]
        [TestCase(100000000, false)]
        public async Task Validate_AcceptsEightDigitNumbersOnly(int ukprn, bool expectedResult)
        {
            var query = new GetProviderStatusQuery(ukprn);
            var repoMock = new Mock<IProvidersReadRepository>();
            repoMock.Setup(x => x.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());
            var sut = new GetProviderStatusQueryValidator(repoMock.Object);

            var result = await sut.ValidateAsync(query);

            Assert.AreEqual(expectedResult, result.IsValid);
        }
    }
}
