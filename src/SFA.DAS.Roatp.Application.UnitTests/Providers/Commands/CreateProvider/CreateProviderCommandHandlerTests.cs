﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.CreateProviderCourse;
using SFA.DAS.Roatp.Application.Providers.Commands.CreateProvider;
using SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Commands.CreateProviderCourse.CreateProviderCourseCommandValidatorTests;
using SFA.DAS.Roatp.Data.Constants;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;
using ProviderType = SFA.DAS.Roatp.Data.Constants.ProviderType;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Commands.CreateProvider
{
    [TestFixture]
    public class CreateProviderCommandHandlerTests
    {
        [Test, RecursiveMoqAutoData]
        public async Task Handle_CreatesProvider(
            [Frozen] Mock<IProvidersWriteRepository> providersWriteRepositoryMock,
            [Frozen] Mock<IProviderRegistrationDetailsWriteRepository> providerRegistrationDetailsWriteRepositoryMock,
            CreateProviderCommandHandler sut,
            CreateProviderCommand command
        )
        {
            var provider = (Provider)command;

            var providerRegistrationDetail = new ProviderRegistrationDetail
            {
                Ukprn = command.Ukprn,
                LegalName = command.LegalName,
                StatusId = OrganisationStatus.Onboarding,
                StatusDate = DateTime.UtcNow,
                OrganisationTypeId = 0,
                ProviderTypeId = ProviderType.Main
            };

            providersWriteRepositoryMock.Setup(p => p.Create(command,command.UserId, command.UserDisplayName, AuditEventTypes.CreateProvider)).ReturnsAsync(provider);
            providerRegistrationDetailsWriteRepositoryMock.Setup(p => p.Create(providerRegistrationDetail, command.UserId, command.UserDisplayName, AuditEventTypes.CreateProviderRegistrationDetail)).ReturnsAsync(providerRegistrationDetail);
            await sut.Handle(command, new CancellationToken());
            providersWriteRepositoryMock.Verify(p => p.Create(It.Is<Provider>(c => c.Ukprn == provider.Ukprn && c.IsImported==false && c.LegalName == provider.LegalName && c.TradingName==c.TradingName && c.Email == null && c.Phone==null && c.Website==null), command.UserId, command.UserDisplayName, AuditEventTypes.CreateProvider));
            providerRegistrationDetailsWriteRepositoryMock.Verify(p => p.Create(It.Is<ProviderRegistrationDetail>(c => c.Ukprn == provider.Ukprn && c.LegalName == provider.LegalName  && c.OrganisationTypeId == 0 && c.StatusId == OrganisationStatus.Onboarding && c.OrganisationTypeId == 0 && c.ProviderTypeId == ProviderType.Main), command.UserId, command.UserDisplayName, AuditEventTypes.CreateProviderRegistrationDetail));
        }
    }
}
