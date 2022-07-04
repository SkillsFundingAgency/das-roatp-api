﻿using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.Locations.Commands.BulkInsert;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkInsert;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers
{
    [TestFixture]
    public class ProviderCourseLocationsBulkInsertControllerTests
    {
        [Test, MoqAutoData]
        public async Task ProviderCourseLocationsBulkInsert_CallsHandler(
            [Frozen] Mock<IMediator> _mediatorMock,
            [Greedy] ProviderCourseLocationsBulkInsertController sut,
            int ukprn, BulkInsertProviderCourseLocationsCommand command)
        {
            await sut.BulkInsertProviderCourseLocations(ukprn, command);

            _mediatorMock.Verify(m => m.Send(It.Is<BulkInsertProviderCourseLocationsCommand>(c => c.Ukprn == ukprn && c.LarsCode == command.LarsCode), It.IsAny<CancellationToken>()));
        }

        [Test, MoqAutoData]
        public async Task ProviderCourseLocationsBulkInsert_MoreThanZeroResults_ReturnsNoContentResponse(
            [Frozen] Mock<IMediator> _mediatorMock,
            [Greedy] ProviderCourseLocationsBulkInsertController sut,
            int ukprn, BulkInsertProviderCourseLocationsCommand command)
        {
            _mediatorMock.Setup(m => m.Send(It.Is<BulkInsertProviderCourseLocationsCommand>(c => c.Ukprn == ukprn && c.LarsCode == command.LarsCode), It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await sut.BulkInsertProviderCourseLocations(ukprn, command);

            _mediatorMock.Verify(m => m.Send(It.Is<BulkInsertProviderCourseLocationsCommand>(c => c.Ukprn == ukprn && c.LarsCode == command.LarsCode), It.IsAny<CancellationToken>()));

            var statusCodeResult = (NoContentResult)result;

            statusCodeResult.Should().NotBeNull();
        }
    }
}
