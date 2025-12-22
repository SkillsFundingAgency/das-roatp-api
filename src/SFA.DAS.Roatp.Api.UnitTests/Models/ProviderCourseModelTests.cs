using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Api.UnitTests.Models
{
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
                Version = "1.1",
                ApprovalBody = "ABC",
                IsRegulatedForProvider = true
            };

            model.AttachCourseDetails(standardLookup.IfateReferenceNumber, standardLookup.Level, standardLookup.Title, standardLookup.Version, standardLookup.ApprovalBody);

            Assert.That(model, Is.Not.Null);
            standardLookup.Title.Should().Be(model.CourseName);
            standardLookup.Level.Should().Be(model.Level);
            standardLookup.Version.Should().Be(model.Version);
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

        [TestCase(true, "Apprenticeship")]
        [TestCase(false, null)]
        [TestCase(false, "NotARealCourseType")]
        public void ProviderCourseModel_ImplicitConversion_FromDomainEntity_SetsProperties(bool hasLocations, string courseTypeString)
        {
            var providerCourse = new Domain.Entities.ProviderCourse
            {
                Id = hasLocations ? 10 : 20,
                LarsCode = hasLocations ? "555" : "999",
                StandardInfoUrl = hasLocations ? "http://standard" : null,
                ContactUsEmail = hasLocations ? "a@b.c" : null,
                ContactUsPhoneNumber = hasLocations ? "000" : null,
                IsApprovedByRegulator = hasLocations ? true : (bool?)null,
                IsImported = hasLocations,
                HasPortableFlexiJobOption = hasLocations,
                Locations = hasLocations ? [new Domain.Entities.ProviderCourseLocation()] : [],
                Standard = courseTypeString == null ? null : new Standard
                {
                    CourseType = courseTypeString,
                    IsRegulatedForProvider = string.Equals(courseTypeString, "Apprenticeship")
                }
            };

            var model = (ProviderCourseModel)providerCourse;

            model.Should().NotBeNull();
            model.ProviderCourseId.Should().Be(providerCourse.Id);

            // Locations
            model.HasLocations.Should().Be(providerCourse.Locations.Count > 0);

            // Standard / CourseType parsing
            if (providerCourse.Standard == null)
            {
                model.IsRegulatedForProvider.Should().BeFalse();
                model.CourseType.Should().BeNull();
            }
            else if (Enum.TryParse<CourseType>(providerCourse.Standard.CourseType, out var parsed))
            {
                model.CourseType.Should().Be(parsed);
                model.IsRegulatedForProvider.Should().Be(providerCourse.Standard.IsRegulatedForProvider);
            }
            else
            {
                model.CourseType.Should().BeNull();
                model.IsRegulatedForProvider.Should().Be(providerCourse.Standard.IsRegulatedForProvider);
            }

            // If present, other properties should map through
            if (providerCourse.StandardInfoUrl != null)
                model.StandardInfoUrl.Should().Be(providerCourse.StandardInfoUrl);
            if (providerCourse.ContactUsEmail != null)
                model.ContactUsEmail.Should().Be(providerCourse.ContactUsEmail);
            if (providerCourse.ContactUsPhoneNumber != null)
                model.ContactUsPhoneNumber.Should().Be(providerCourse.ContactUsPhoneNumber);
            if (providerCourse.IsApprovedByRegulator.HasValue)
                model.IsApprovedByRegulator.Should().Be(providerCourse.IsApprovedByRegulator);
            model.IsImported.Should().Be(providerCourse.IsImported);
            model.HasPortableFlexiJobOption.Should().Be(providerCourse.HasPortableFlexiJobOption);
        }

        [Test]
        public void ProviderCourseModel_ImplicitConversion_WhenEntityIsNull(
            )
        {
            Domain.Entities.ProviderCourse providerCourse = null;

            var model = (ProviderCourseModel)providerCourse;
            model.Should().BeNull();
        }
    }
}
