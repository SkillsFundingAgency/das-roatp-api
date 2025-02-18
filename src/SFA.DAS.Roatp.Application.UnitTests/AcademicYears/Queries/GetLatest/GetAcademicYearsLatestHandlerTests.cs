using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.AcademicYears.Queries.GetLatest;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.AcademicYears.Queries.GetLatest;

[TestFixture]
public class GetAcademicYearsLatestHandlerTests
{
    [Test, RecursiveMoqAutoData]
    public async Task Handle_ReturnsResult(
        List<Domain.Entities.NationalQar> nationalQars,
        GetAcademicYearsLatestQuery query,
        GetAcademicYearsLatestQueryHandler sut,
        CancellationToken cancellationToken)
    {
        var response = await sut.Handle(query, cancellationToken);
        response.Should().NotBeNull();
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_ReturnsResult_CheckingQarPeriod(
        List<Domain.Entities.NationalQar> nationalQars,
        [Frozen] Mock<INationalQarReadRepository> nationalQarReadRepositoryMock,
        GetAcademicYearsLatestQuery query,
        GetAcademicYearsLatestQueryHandler sut,
        CancellationToken cancellationToken)
    {
        nationalQarReadRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(nationalQars);
        var response = await sut.Handle(query, cancellationToken);

        response.Should().NotBeNull();
        response.QarPeriod.Should().Be(nationalQars.Max(x => x.TimePeriod));
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_ReturnsResult_CheckingReviewPeriod_NoFeedbackPeriods(
        List<Domain.Entities.NationalQar> nationalQars,
        [Frozen] Mock<IProviderEmployerStarsReadRepository> providerEmployerStarsReadRepository,
        GetAcademicYearsLatestQuery query,
        GetAcademicYearsLatestQueryHandler sut,
        CancellationToken cancellationToken)
    {
        var noTimePeriods = new List<string>();
        providerEmployerStarsReadRepository.Setup(r => r.GetTimePeriods()).ReturnsAsync(noTimePeriods);

        string reviewPeriodCalculated = string.Concat(DateTime.UtcNow.AddMonths(-19).Year.ToString().AsSpan(2, 2),
                                                    DateTime.UtcNow.AddMonths(-7).Year.ToString().AsSpan(2, 2));

        var response = await sut.Handle(query, cancellationToken);

        response.Should().NotBeNull();
        response.ReviewPeriod.Should().Be(reviewPeriodCalculated);
    }


    [Test, RecursiveMoqAutoData]
    public async Task Handle_ReturnsResult_CheckingReviewPeriod_WithFeedbackPeriods(
        List<Domain.Entities.NationalQar> nationalQars,
        [Frozen] Mock<IProviderEmployerStarsReadRepository> providerEmployerStarsReadRepository,
        GetAcademicYearsLatestQuery query,
        GetAcademicYearsLatestQueryHandler sut,
        CancellationToken cancellationToken)
    {
        var maxTimePeriodExpected = "2324";

        var maxTimePeriod = $"AY{maxTimePeriodExpected}";
        var middleTimePeriod = "AY2223";
        var lowestTimePeriod = "AY2122";


        var timePeriods = new List<string>
        {
            middleTimePeriod,
            maxTimePeriod,
            lowestTimePeriod
        };

        providerEmployerStarsReadRepository.Setup(r => r.GetTimePeriods()).ReturnsAsync(timePeriods);

        var response = await sut.Handle(query, cancellationToken);

        response.Should().NotBeNull();
        response.ReviewPeriod.Should().Be(maxTimePeriodExpected);
    }
}
