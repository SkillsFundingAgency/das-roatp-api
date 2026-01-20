using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Api.UnitTests.Models;

[TestFixture]
public class ProviderCourseModelTests
{
    [TestCase(true)]
    [TestCase(false)]
    public void ProviderCourseOperator_ReturnsProviderCourseModel(bool hasPortableFlexiJobOption)
    {
        var course = new Domain.Entities.ProviderCourse() { LarsCode = "1", HasPortableFlexiJobOption = hasPortableFlexiJobOption, Standard = new Standard() };
        var model = (ProviderCourseModel)course;

        Assert.That(model, Is.Not.Null);
        Assert.That(model.LarsCode, Is.EqualTo(course.LarsCode));
        Assert.That(model.HasPortableFlexiJobOption, Is.EqualTo(hasPortableFlexiJobOption));
    }

    [Test]
    public void ProviderCourseOperator_UpdateCourseInjectsExpectedValues()
    {
        var course = new Domain.Entities.ProviderCourse() { LarsCode = "1", Standard = new Standard() };
        var model = (ProviderCourseModel)course;

        var standardLookup = new Standard
        {
            Title = "course title",
            Level = 1,
            ApprovalBody = "ABC",
            IsRegulatedForProvider = true
        };

        model.AttachCourseDetails(standardLookup.IfateReferenceNumber, standardLookup.Level, standardLookup.Title, standardLookup.ApprovalBody);

        Assert.That(model, Is.Not.Null);
        standardLookup.Title.Should().Be(model.CourseName);
        standardLookup.Level.Should().Be(model.Level);
        standardLookup.ApprovalBody.Should().Be(model.ApprovalBody);
    }

    [TestCase(1, true)]
    [TestCase(0, false)]
    public void ProviderCourseOperator_SetsHasLocation(int numberOfLocations, bool expected)
    {
        var locations = new List<ProviderCourseLocation>();
        for (int i = 0; i < numberOfLocations; i++)
        {
            locations.Add(new ProviderCourseLocation());
        }

        var course = new Domain.Entities.ProviderCourse { Locations = locations, Standard = new Standard() };
        var model = (ProviderCourseModel)course;

        Assert.That(model, Is.Not.Null);
        Assert.AreEqual(expected, model.HasLocations);
    }

    [Test]
    public void ProviderCourseModel_ImplicitConversion_WhenEntityPopulated_SetsProperties()
    {
        var providerCourse = new Domain.Entities.ProviderCourse
        {
            Id = 10,
            LarsCode = "555",
            StandardInfoUrl = "http://standard",
            ContactUsEmail = "a@b.c",
            ContactUsPhoneNumber = "000",
            IsApprovedByRegulator = true,
            IsImported = true,
            HasPortableFlexiJobOption = true,
            Locations = [new ProviderCourseLocation()],
            Standard = new Standard
            {
                CourseType = CourseType.Apprenticeship,
                IsRegulatedForProvider = true
            }
        };

        var model = (ProviderCourseModel)providerCourse;

        model.Should().NotBeNull();
        model.ProviderCourseId.Should().Be(providerCourse.Id);
        model.HasLocations.Should().BeTrue();
        model.StandardInfoUrl.Should().Be(providerCourse.StandardInfoUrl);
        model.ContactUsEmail.Should().Be(providerCourse.ContactUsEmail);
        model.ContactUsPhoneNumber.Should().Be(providerCourse.ContactUsPhoneNumber);
        model.IsApprovedByRegulator.Should().Be(providerCourse.IsApprovedByRegulator.Value);
        model.HasPortableFlexiJobOption.Should().Be(providerCourse.HasPortableFlexiJobOption);
        model.CourseType.Should().Be(CourseType.Apprenticeship);
        model.IsRegulatedForProvider.Should().Be(providerCourse.Standard.IsRegulatedForProvider);
    }

    [Test]
    public void ProviderCourseModel_ImplicitConversion_WhenStandardIsNull_SetsDefaults()
    {
        var providerCourse = new Domain.Entities.ProviderCourse
        {
            Id = 20,
            LarsCode = "999",
            StandardInfoUrl = null,
            ContactUsEmail = null,
            ContactUsPhoneNumber = null,
            IsApprovedByRegulator = null,
            IsImported = false,
            HasPortableFlexiJobOption = false,
            Locations = [],
            Standard = null
        };

        var model = (ProviderCourseModel)providerCourse;

        model.Should().NotBeNull();
        model.ProviderCourseId.Should().Be(providerCourse.Id);
        model.HasLocations.Should().BeFalse();
        model.StandardInfoUrl.Should().BeNull();
        model.ContactUsEmail.Should().BeNull();
        model.ContactUsPhoneNumber.Should().BeNull();
        model.IsApprovedByRegulator.Should().BeNull();
        model.HasPortableFlexiJobOption.Should().Be(providerCourse.HasPortableFlexiJobOption);
        model.CourseType.Should().BeNull();
        model.IsRegulatedForProvider.Should().BeFalse();
    }

    [Test]
    public void ProviderCourseModel_ImplicitConversion_WhenEntityIsNull()
    {
        Domain.Entities.ProviderCourse providerCourse = null;

        var model = (ProviderCourseModel)providerCourse;
        model.Should().BeNull();
    }
}