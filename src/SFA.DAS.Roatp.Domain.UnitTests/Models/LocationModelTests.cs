using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Domain.UnitTests.Models;

public sealed class LocationModelTests
{
    [Test]
    [RecursiveMoqAutoData]
    public void ImplicitOperator_ShouldConvertCourseProviderDetailsModel_ToLocationModel(CourseProviderDetailsModel source)
    {
        LocationModel sut = source;

        Assert.Multiple(() =>
        {
            Assert.That(sut, Is.Not.Null);
            Assert.That(sut.Ordering, Is.EqualTo(source.Ordering));
            Assert.That(sut.AtEmployer, Is.EqualTo(source.AtEmployer));
            Assert.That(sut.BlockRelease, Is.EqualTo(source.BlockRelease));
            Assert.That(sut.DayRelease, Is.EqualTo(source.DayRelease));
            Assert.That(sut.LocationType, Is.EqualTo(source.LocationType));
            Assert.That(sut.CourseLocation, Is.EqualTo(source.CourseLocation));
            Assert.That(sut.AddressLine1, Is.EqualTo(source.AddressLine1));
            Assert.That(sut.AddressLine2, Is.EqualTo(source.AddressLine2));
            Assert.That(sut.Town, Is.EqualTo(source.Town));
            Assert.That(sut.County, Is.EqualTo(source.County));
            Assert.That(sut.Postcode, Is.EqualTo(source.Postcode));
            Assert.That(sut.CourseDistance, Is.EqualTo(source.CourseDistance));
        });
    }
}
