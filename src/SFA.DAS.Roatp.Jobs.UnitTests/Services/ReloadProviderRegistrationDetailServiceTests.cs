using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels;
using SFA.DAS.Roatp.Jobs.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Services;

[TestFixture]
public class ReloadProviderRegistrationDetailServiceTests
{
    [Test]
    [MoqAutoData]
    public async Task ReloadProviderRegistrationDetails_OnApiError_ThrowsInvalidOperationException(
        [Frozen] Mock<IReloadProviderRegistrationDetailsRepository> repositoryMock,
        [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
        [Greedy] ReloadProviderRegistrationDetailService sut)
    {
        apiClientMock.Setup(a => a.Get<List<ProviderRegistrationDetail>>("lookup/registered-providers")).ReturnsAsync((false, null));

        Func<Task> action = () => sut.ReloadProviderRegistrationDetails();

        await action.Should().ThrowAsync<InvalidOperationException>();
    }

    [Test]
    [RecursiveMoqAutoData]
    public async Task ReloadProviderRegistrationDetails_OnApiSuccess_CallsRepositoryReloadMethod(
        [Frozen] Mock<IReloadProviderRegistrationDetailsRepository> repositoryMock,
        [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
        [Greedy] ReloadProviderRegistrationDetailService sut,
        List<RegisteredProviderModel> data)
    {
        apiClientMock.Setup(a => a.Get<List<RegisteredProviderModel>>("lookup/registered-providers")).ReturnsAsync((true, data));

        await sut.ReloadProviderRegistrationDetails();

        repositoryMock.Verify(r => r.ReloadRegisteredProviders(It.Is<List<ProviderRegistrationDetail>>(c => c.Count == data.Count), It.IsAny<DateTime>()));
    }
}
