using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Courses.Queries.GetCourseProviderDetails;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Models;

[TestFixture]
public class GetCourseProviderDetailsResultV1ModelTests
{
    [Test, MoqAutoData]
    public void ImplicitOperator_WithValidSource_MapsAllFields(
        GetCourseProviderDetailsQueryResult source)
    {
        // Act
        GetCourseProviderDetailsResultV1Model result = source;

        // Assert
        result.Should().NotBeNull();

        result.Should().BeEquivalentTo(source, options => options
            .ExcludingMissingMembers()
            .Excluding(p => p.HasOnlineDeliveryOption)
            .Excluding(p => p.CourseType)
            .Excluding(p => p.Locations)
            .Excluding(p => p.LarsCode));

        var expectedLarsCode = int.TryParse(source.LarsCode, out var parsed) ? parsed : 0;
        result.LarsCode.Should().Be(expectedLarsCode);
    }

    [Test]
    public void ImplicitOperator_WithNullSource_ReturnsNull()
    {
        GetCourseProviderDetailsQueryResult source = null;

        GetCourseProviderDetailsResultV1Model result = source;

        result.Should().BeNull();
    }

    [Test, MoqAutoData]
    public void ImplicitOperator_WhenLarsCodeIsNonNumeric_SetsZero(GetCourseProviderDetailsQueryResult source)
    {
        source.LarsCode = "ABC";

        GetCourseProviderDetailsResultV1Model result = source;

        result.LarsCode.Should().Be(0);
    }

    [Test, MoqAutoData]
    public void ImplicitOperator_WhenAddressIsNull_ThrowsNullReferenceException(GetCourseProviderDetailsQueryResult source)
    {
        source.Address = null;

        Assert.Throws<NullReferenceException>(() =>
        {
            GetCourseProviderDetailsResultV1Model r = source;
            _ = r.Address;
        });
    }

    [Test, MoqAutoData]
    public void ImplicitOperator_WhenContactIsNull_ThrowsNullReferenceException(GetCourseProviderDetailsQueryResult source)
    {
        source.Contact = null;

        Assert.Throws<NullReferenceException>(() =>
        {
            GetCourseProviderDetailsResultV1Model r = source;
            _ = r.Contact;
        });
    }

    [Test, MoqAutoData]
    public void ImplicitOperator_WhenQarIsNull_ThrowsNullReferenceException(GetCourseProviderDetailsQueryResult source)
    {
        source.QAR = null;

        Assert.Throws<NullReferenceException>(() =>
        {
            GetCourseProviderDetailsResultV1Model r = source;
            _ = r.QAR;
        });
    }

    [Test, MoqAutoData]
    public void ImplicitOperator_WhenReviewsIsNull_ThrowsNullReferenceException(GetCourseProviderDetailsQueryResult source)
    {
        source.Reviews = null;

        Assert.Throws<NullReferenceException>(() =>
        {
            GetCourseProviderDetailsResultV1Model r = source;
            _ = r.Reviews;
        });
    }
}