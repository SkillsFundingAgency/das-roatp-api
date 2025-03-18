using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.Functions;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Functions;

public class DeleteExpiredShortlistsFunctionTests
{
    [Test, AutoData]
    public async Task DeleteExpiredShortlistsFunction_InvokesOuterApi(CancellationToken cancellationToken)
    {
        Mock<ICourseManagementOuterApiClient> mockClient = new();
        DeleteExpiredShortlistsFunction sut = new(Mock.Of<ILogger<DeleteExpiredShortlistsFunction>>(), mockClient.Object);

        await sut.Run(null, cancellationToken);

        mockClient.Verify(c => c.Delete(DeleteExpiredShortlistsFunction.DeleteExpiredShortlistsUri, cancellationToken), Times.Once);
    }
}
