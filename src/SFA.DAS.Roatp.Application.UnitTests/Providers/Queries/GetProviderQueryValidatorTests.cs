using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Queries.GetProviderCourseLocations;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProvider;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Queries
{
    [TestFixture]
    public class GetProviderQueryValidatorTests
    {
        [TestCase(10000001,  true)]
        [TestCase(10000000,  false)]
        [TestCase(100000000,  false)]
        public async Task Validate_AcceptsEightDigitNumbersOnly(int ukprn, bool expectedResult)
        {
            var query = new GetProviderQuery(ukprn);
            var repoMock = new Mock<IProvidersReadRepository>();
            repoMock.Setup(x => x.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider()); 
            var sut = new GetProviderQueryValidator(repoMock.Object);

            var result = await sut.ValidateAsync(query);

            Assert.AreEqual(expectedResult, result.IsValid);
        }
    }
}
