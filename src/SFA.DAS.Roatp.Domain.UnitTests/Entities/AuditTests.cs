using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using ProviderCourse = SFA.DAS.Roatp.Domain.Entities.ProviderCourse;

namespace SFA.DAS.Roatp.Domain.UnitTests.Entities
{
    [TestFixture]
    public class AuditTests
    {
        private Activity unitTestActivity;
        [SetUp]
        public void Before_Each_Test()
        {
            unitTestActivity = new Activity("UnitTest").Start();
        }

        [TearDown]
        public void Cleanup()
        { 
            unitTestActivity.Stop(); 
        }

        [Test]
        public void DefaultConstructor_ReturnsEmptyAudit()
        {
            Audit audit = new();

            audit.Should().NotBeNull();
        }

        [Test, RecursiveMoqAutoData]
        public void Constructor_SetsAuditProperties(ProviderCourse providerCourseInitialState, ProviderCourse providerCourseUpdatedState, string userId, string userDisplayName, string userAction)
        {
            Audit audit = new(typeof(ProviderCourse).Name, providerCourseInitialState.Id.ToString(), userId, userDisplayName, userAction, providerCourseInitialState, providerCourseUpdatedState);

            JsonSerializerOptions jsonSerializerOptions = new()
            { ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve };

            var jsonStringInitialState = JsonSerializer.Serialize(providerCourseInitialState, jsonSerializerOptions);
            var jsonStringUpdatedState = JsonSerializer.Serialize(providerCourseUpdatedState, jsonSerializerOptions);


            audit.Should().NotBeNull();
            audit.Id.Should().Be(0);
            audit.CorrelationId.Should().NotBeEmpty();
            audit.EntityType.Should().Be(typeof(ProviderCourse).Name);
            audit.EntityId.Should().Be(providerCourseInitialState.Id.ToString());
            audit.UserId.Should().Be(userId);
            audit.UserDisplayName.Should().Be(userDisplayName);
            audit.UserAction.Should().Be(userAction);
            audit.AuditDate.Date.Should().Be(DateTime.Now.Date);
            audit.InitialState.Should().Be(jsonStringInitialState);
            audit.UpdatedState.Should().Be(jsonStringUpdatedState);
        }

        [Test, RecursiveMoqAutoData]
        public void Constructor_SetsAuditPropertiesForListOfObjects(List<ProviderCourse> providerCourseInitialState, List<ProviderCourse> providerCourseUpdatedState, string userId, string userDisplayName, string userAction)
        {
            Audit audit = new(typeof(ProviderCourse).Name, providerCourseInitialState.FirstOrDefault().ProviderId.ToString(), userId, userDisplayName, userAction, providerCourseInitialState, providerCourseUpdatedState);

            JsonSerializerOptions jsonSerializerOptions = new()
            { ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve };

            var jsonStringInitialState = JsonSerializer.Serialize(providerCourseInitialState, jsonSerializerOptions);
            var jsonStringUpdatedState = JsonSerializer.Serialize(providerCourseUpdatedState, jsonSerializerOptions);


            audit.Should().NotBeNull();
            audit.Id.Should().Be(0);
            audit.CorrelationId.Should().NotBeEmpty();
            audit.EntityType.Should().Be(typeof(ProviderCourse).Name);
            audit.EntityId.Should().Be(providerCourseInitialState.FirstOrDefault().ProviderId.ToString());
            audit.UserId.Should().Be(userId);
            audit.UserDisplayName.Should().Be(userDisplayName);
            audit.UserAction.Should().Be(userAction);
            audit.AuditDate.Date.Should().Be(DateTime.Now.Date);
            audit.InitialState.Should().Be(jsonStringInitialState);
            audit.UpdatedState.Should().Be(jsonStringUpdatedState);
        }
    }
}