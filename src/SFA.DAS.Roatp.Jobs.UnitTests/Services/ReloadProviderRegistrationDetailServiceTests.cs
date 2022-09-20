﻿using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.Services;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Services
{
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
        [MoqAutoData]
        public async Task ReloadProviderRegistrationDetails_OnApiSuccess_CallsRepositoryReloadMethod(
            [Frozen] Mock<IReloadProviderRegistrationDetailsRepository> repositoryMock,
            [Frozen] Mock<IImportAuditWriteRepository> auditRepositoryMock,
            [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
            [Greedy] ReloadProviderRegistrationDetailService sut,
            List<ProviderRegistrationDetail> data)
        {
            apiClientMock.Setup(a => a.Get<List<ProviderRegistrationDetail>>("lookup/registered-providers")).ReturnsAsync((true, data));

            await sut.ReloadProviderRegistrationDetails();

            repositoryMock.Verify(r => r.ReloadRegisteredProviders(data));
            auditRepositoryMock.Verify(x=>x.Insert(It.IsAny<ImportAudit>()));
        }
    }
}
