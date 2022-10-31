using NUnit.Framework;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetProviderDetailsForCourse;

[TestFixture]
public class LocationDetailTests
{
    [Test, RecursiveMoqAutoData]
    public void Operator_PopulatesModelFromEntity(ProviderLocationDetailsWithDistance providerLocationDetailsWithDistance)
    {
        var model = (LocationDetail)providerLocationDetailsWithDistance;

        Assert.That(model, Is.Not.Null);
        Assert.AreEqual(providerLocationDetailsWithDistance.LocationType, model.LocationType);
        Assert.AreEqual(providerLocationDetailsWithDistance.HasBlockReleaseDeliveryOption??false, model.BlockRelease); 
        Assert.AreEqual(providerLocationDetailsWithDistance.HasDayReleaseDeliveryOption??false, model.DayRelease); 
        Assert.AreEqual(providerLocationDetailsWithDistance.RegionName, model.RegionName); 
        Assert.AreEqual(providerLocationDetailsWithDistance.SubregionName, model.SubRegionName); 
        Assert.AreEqual(providerLocationDetailsWithDistance.Distance, model.Distance); 
        Assert.AreEqual(providerLocationDetailsWithDistance.Latitude, model.Latitude); 
        Assert.AreEqual(providerLocationDetailsWithDistance.Longitude, model.Longitude); 
        Assert.AreEqual(providerLocationDetailsWithDistance.AddressLine1, model.Address?.Address1); 
        Assert.AreEqual(providerLocationDetailsWithDistance.Addressline2, model.Address?.Address2); 
        Assert.AreEqual(providerLocationDetailsWithDistance.Town, model.Address?.Town); 
        Assert.AreEqual(providerLocationDetailsWithDistance.Postcode, model.Address?.Postcode);
    }
}