using NUnit.Framework;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetProviderDetailsForCourse;

[TestFixture]
public class CourseLocationModelTests
{
    [Test, RecursiveMoqAutoData]
    public void Operator_PopulatesModelFromEntity(ProviderCourseLocationDetailsModel providerCourseLocationDetailsModel)
    {
        var model = (CourseLocationModel)providerCourseLocationDetailsModel;

        Assert.That(model, Is.Not.Null);
        Assert.AreEqual(providerCourseLocationDetailsModel.LocationType, model.LocationType);
        Assert.AreEqual(providerCourseLocationDetailsModel.HasBlockReleaseDeliveryOption??false, model.BlockRelease); 
        Assert.AreEqual(providerCourseLocationDetailsModel.HasDayReleaseDeliveryOption??false, model.DayRelease); 
        Assert.AreEqual(providerCourseLocationDetailsModel.RegionName, model.RegionName); 
        Assert.AreEqual(providerCourseLocationDetailsModel.SubregionName, model.SubRegionName); 
        Assert.AreEqual(providerCourseLocationDetailsModel.Distance, model.ProviderLocationDistanceInMiles); 
        Assert.AreEqual(providerCourseLocationDetailsModel.Latitude, model.Latitude); 
        Assert.AreEqual(providerCourseLocationDetailsModel.Longitude, model.Longitude); 
        Assert.AreEqual(providerCourseLocationDetailsModel.AddressLine1, model.Address?.Address1); 
        Assert.AreEqual(providerCourseLocationDetailsModel.Addressline2, model.Address?.Address2); 
        Assert.AreEqual(providerCourseLocationDetailsModel.Town, model.Address?.Town); 
        Assert.AreEqual(providerCourseLocationDetailsModel.Postcode, model.Address?.Postcode);
    }
}