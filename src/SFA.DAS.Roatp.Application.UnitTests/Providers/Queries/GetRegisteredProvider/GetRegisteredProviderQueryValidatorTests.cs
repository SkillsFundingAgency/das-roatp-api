using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Providers.Queries.GetRegisteredProvider;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Queries.GetRegisteredProvider;

public class GetRegisteredProviderQueryValidatorTests
{
    [TestCase(10000001, true)]
    [TestCase(10000000, false)]
    [TestCase(100000000, false)]
    public async Task Validate_AcceptsEightDigitNumbersOnly(int ukprn, bool expectedResult)
    {
        var query = new GetRegisteredProviderQuery(ukprn);
        var repoMock = new Mock<IProvidersReadRepository>();
        repoMock.Setup(x => x.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());
        var sut = new GetRegisteredProviderQueryValidator(repoMock.Object);

        var result = await sut.ValidateAsync(query);

        result.IsValid.Should().Be(expectedResult);
    }
}
