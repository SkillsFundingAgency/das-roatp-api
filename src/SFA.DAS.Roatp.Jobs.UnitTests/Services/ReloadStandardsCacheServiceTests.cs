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
using SFA.DAS.Roatp.Jobs.ApiModels.Lookup;
using SFA.DAS.Roatp.Jobs.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Services
{
    [TestFixture]
    public class ReloadStandardsCacheServiceTests
    {
        [Test]
        [MoqAutoData]
        public async Task ReloadStandardsCache_GetsDataFromOuterApi_CallsRepositoryToSaveIt(
            [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
            [Frozen] Mock<IReloadStandardsRepository> repositoryMock,
            [Frozen] Mock<IImportAuditWriteRepository> auditRepositoryMock,
            [Greedy] ReloadStandardsCacheService sut,
            StandardList data)
        {
            data.Standards.ForEach(s => s.Level = 1);
            apiClientMock.Setup(c => c.Get<StandardList>(It.IsAny<string>())).ReturnsAsync((true, data));

            await sut.ReloadStandardsCache();

            repositoryMock.Verify(r => r.ReloadStandards(It.Is<List<Domain.Entities.Standard>>(list => list.Count == data.Standards.Count)));
            auditRepositoryMock.Verify(x => x.Insert(It.IsAny<ImportAudit>()));
        }

        [Test]
        [MoqAutoData]
        public async Task ReloadStandardsCache_GetsNoDataFromOuterApi_CallsRepositoryToSaveIt(
            [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
            [Frozen] Mock<IReloadStandardsRepository> repositoryMock,
            [Frozen] Mock<IImportAuditWriteRepository> auditRepositoryMock,
            [Greedy] ReloadStandardsCacheService sut)
        {
            apiClientMock.Setup(c => c.Get<StandardList>(It.IsAny<string>())).ReturnsAsync((true, new StandardList()));

            Func<Task> action = () => sut.ReloadStandardsCache();

            await action.Should().ThrowAsync<InvalidOperationException>();

            repositoryMock.Verify(r => r.ReloadStandards(It.IsAny<List<Domain.Entities.Standard>>()), Times.Never);
            auditRepositoryMock.Verify(x => x.Insert(It.IsAny<ImportAudit>()), Times.Never);
        }

        [Test]
        [MoqAutoData]
        public async Task ReloadStandardsCache_GetUnsuccessfulResponseFromOuterApi_CallsRepositoryToSaveIt(
            [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
            [Frozen] Mock<IReloadStandardsRepository> repositoryMock,
            [Frozen] Mock<IImportAuditWriteRepository> auditRepositoryMock,
            [Greedy] ReloadStandardsCacheService sut)
        {
            apiClientMock.Setup(c => c.Get<StandardList>(It.IsAny<string>())).ReturnsAsync((false, null));

            Func<Task> action = () => sut.ReloadStandardsCache();

            await action.Should().ThrowAsync<InvalidOperationException>();

            repositoryMock.Verify(r => r.ReloadStandards(It.IsAny<List<Domain.Entities.Standard>>()), Times.Never);
            auditRepositoryMock.Verify(x => x.Insert(It.IsAny<ImportAudit>()), Times.Never);
        }
    }
}
