using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Jobs.Functions;
using SFA.DAS.Roatp.Jobs.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Functions
{
    [TestFixture]
    public class LoadAllProviderAddressesFunctionTests
    {
        [Test]
        public async Task Run_InvokesService()
        {
            var serviceMock = new Mock<ILoadUkrlpAddressesService>();
            var sut = new LoadAllProviderAddressesFunction(serviceMock.Object);

            await sut.Run(It.IsAny<HttpRequest>(),Mock.Of<ILogger>());

            serviceMock.Verify(s => s.LoadUkrlpAddresses());
        }
    }
}
