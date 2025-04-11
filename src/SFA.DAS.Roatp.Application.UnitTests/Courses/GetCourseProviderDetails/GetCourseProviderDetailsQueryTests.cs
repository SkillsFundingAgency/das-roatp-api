using NUnit.Framework;
using SFA.DAS.Roatp.Application.Courses.Queries.GetCourseProviderDetails;
using SFA.DAS.Testing.AutoFixture;
using System;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetCourseProviderDetails;

public sealed class GetCourseProviderDetailsQueryTests
{
    [Test]
    [MoqAutoData]
    public void Constructor_SetsAllProperties_WhenValidParametersAreProvided(
        int ukprn,
        int larsCode,
        decimal? longitude,
        decimal? latitude,
        string location,
        Guid shortlistUserId
    )
    {
        GetCourseProviderDetailsQuery sut = new GetCourseProviderDetailsQuery(
            ukprn,
            larsCode,
            shortlistUserId,
            location,
            longitude,
            latitude
        );

        Assert.Multiple(() =>
        {
            Assert.That(sut.Ukprn, Is.EqualTo(ukprn));
            Assert.That(sut.LarsCode, Is.EqualTo(larsCode));
            Assert.That(sut.Longitude, Is.EqualTo(longitude));
            Assert.That(sut.Latitude, Is.EqualTo(latitude));
            Assert.That(sut.ShortlistUserId, Is.EqualTo(shortlistUserId));
            Assert.That(sut.Location, Is.EqualTo(location));
        });
    }

    [Test]
    [MoqAutoData]
    public void Constructor_SetsLocationToNull_WhenInputIsEmptyOrWhitespace(
        int ukprn,
        int larsCode,
        decimal? longitude,
        decimal? latitude,
        string location,
        Guid shortlistUserId
    )
    {
        GetCourseProviderDetailsQuery sut = new GetCourseProviderDetailsQuery(
            ukprn,
            larsCode,
            shortlistUserId,
            string.Empty,
            longitude,
            latitude
        );

        Assert.That(sut.Location, Is.Null);
    }

    [Test]
    [MoqAutoData]
    public void Constructor_ShouldTrimLocation_WhenWhitespaceIsPresent(
        int ukprn,
        int larsCode,
        decimal? longitude,
        decimal? latitude,
        Guid shortlistUserId
    )
    {
        string location = " location ";
        string expected = "location";

        GetCourseProviderDetailsQuery sut = new GetCourseProviderDetailsQuery(
            ukprn,
            larsCode,
            shortlistUserId,
            location,
            longitude,
            latitude
        );

        Assert.That(sut.Location, Is.EqualTo(expected));
    }
}
