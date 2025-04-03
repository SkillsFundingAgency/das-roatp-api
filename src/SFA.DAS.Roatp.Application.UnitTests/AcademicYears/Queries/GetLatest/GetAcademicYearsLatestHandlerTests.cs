using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.AcademicYears.Queries.GetLatest;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
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

    [Test]
    [InlineAutoData("2324", "2223", "2122", "2324")]
    [InlineAutoData("2324", "2122", "2223", "2324")]
    [InlineAutoData("2223", "2324", "2122", "2324")]
    [InlineAutoData("2223", "2122", "2324", "2324")]
    [InlineAutoData("2122", "2223", "2324", "2324")]
    [InlineAutoData("2122", "2324", "2223", "2324")]
    [InlineAutoData("0022", "2728", "2223", "2728")]
    [InlineAutoData("2122", "3031", "2989", "3031")]
    public async Task Handle_MultipleNationalQars_ReturnsLatestQarPeriod(
        string timePeriod1, string timePeriod2, string timePeriod3, string expectedTimePeriod,
        [Frozen] Mock<INationalQarReadRepository> nationalQarReadRepositoryMock,
        [Frozen] Mock<IProviderEmployerStarsReadRepository> providerEmployerStarsReadRepositoryMock,
        GetAcademicYearsLatestQuery query,
        CancellationToken cancellationToken)
    {
        var nationalQars = new List<NationalQar>
        {
            new() { TimePeriod = timePeriod1 },
            new() { TimePeriod = timePeriod2 },
            new() { TimePeriod = timePeriod3 }
        };

        nationalQarReadRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(nationalQars);

        var sut = new GetAcademicYearsLatestQueryHandler(nationalQarReadRepositoryMock.Object, providerEmployerStarsReadRepositoryMock.Object);

        var response = await sut.Handle(query, cancellationToken);

        response.Should().NotBeNull();
        response.QarPeriod.Should().Be(expectedTimePeriod);
    }


    [Test, RecursiveMoqAutoData]
    public async Task Handle_CheckingReviewPeriodWithNoFeedbackPeriods_ReturnsCalculatedReviewPeriod(
        List<Domain.Entities.NationalQar> nationalQars,
        [Frozen] Mock<IProviderEmployerStarsReadRepository> providerEmployerStarsReadRepository,
        GetAcademicYearsLatestQuery query,
        GetAcademicYearsLatestQueryHandler sut,
        CancellationToken cancellationToken)
    {
        var noTimePeriods = new List<string>();
        providerEmployerStarsReadRepository.Setup(r => r.GetTimePeriods()).ReturnsAsync(noTimePeriods);

        var response = await sut.Handle(query, cancellationToken);

        response.Should().NotBeNull();
        response.ReviewPeriod.Should().NotBeNull();
        response.ReviewPeriod.Length.Should().Be(4);

    }


    [Test, RecursiveMoqAutoData]
    public async Task Handle_MultipleReviewPeriods_ReturnsMaximumTimePeriod(
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
