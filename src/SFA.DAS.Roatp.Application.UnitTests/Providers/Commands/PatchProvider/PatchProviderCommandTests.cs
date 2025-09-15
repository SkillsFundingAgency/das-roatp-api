using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.PatchProviderCourse;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Commands.PatchProvider
{
    [TestFixture]
    public class PatchProviderCommandTests
    {
        private const string Replace = "replace";
        private const string ContactUsEmail = "ContactUsEmail";
        private const string ContactUsPhoneNumber = "ContactUsPhoneNumber";
        private const string StandardInfoUrl = "StandardInfoUrl";
        private const string IsApprovedByRegulator = "IsApprovedByRegulator";

        [Test]
        public void Command_PatchContainsStandardInfoUrl_StandardInfoUrlIsSet()
        {
            var ukprn = 10000001;
            var larsCode = 1;
            var testValue = "value";
            var patchCommand = new JsonPatchDocument<PatchProviderCourse>();
            patchCommand.Operations.Add(new Operation<PatchProviderCourse> { op = Replace, path = StandardInfoUrl, value = testValue });

            var command = new PatchProviderCourseCommand
            {
                Ukprn = ukprn,
                LarsCode = larsCode,
                Patch = patchCommand
            };

            testValue.Should().Be(command.StandardInfoUrl);
            command.IsPresentStandardInfoUrl.Should().BeTrue();
        }

        [Test]
        public void Command_PatchContainsContactUsEmail_ContactUsEmailIsSet()
        {
            var ukprn = 10000001;
            var larsCode = 1;
            var testValue = "value";
            var patchCommand = new JsonPatchDocument<PatchProviderCourse>();
            patchCommand.Operations.Add(new Operation<PatchProviderCourse> { op = Replace, path = ContactUsEmail, value = testValue });

            var command = new PatchProviderCourseCommand
            {
                Ukprn = ukprn,
                LarsCode = larsCode,
                Patch = patchCommand
            };

            testValue.Should().Be(command.ContactUsEmail);
            command.IsPresentContactUsEmail.Should().BeTrue();
        }

        [Test]
        public void Command_PatchContainsContactUsPhoneNumber_ContactUsPhoneNumberIsSet()
        {
            var ukprn = 10000001;
            var larsCode = 1;
            var testValue = "value";
            var patchCommand = new JsonPatchDocument<PatchProviderCourse>();
            patchCommand.Operations.Add(new Operation<PatchProviderCourse> { op = Replace, path = ContactUsPhoneNumber, value = testValue });

            var command = new PatchProviderCourseCommand
            {
                Ukprn = ukprn,
                LarsCode = larsCode,
                Patch = patchCommand
            };

            testValue.Should().Be(command.ContactUsPhoneNumber);
            command.IsPresentContactUsPhoneNumber.Should().BeTrue();
        }


        [Test]
        public void Command_PatchContainsIsApprovedByRegulator_IsApprovedByRegulatorIsSet()
        {
            var ukprn = 10000001;
            var larsCode = 1;
            var testValue = "true";
            var patchCommand = new JsonPatchDocument<PatchProviderCourse>();
            patchCommand.Operations.Add(new Operation<PatchProviderCourse> { op = Replace, path = IsApprovedByRegulator, value = testValue });

            var command = new PatchProviderCourseCommand
            {
                Ukprn = ukprn,
                LarsCode = larsCode,
                Patch = patchCommand
            };

            command.IsApprovedByRegulator.Should().BeTrue();
            command.IsPresentIsApprovedByRegulator.Should().BeTrue();
        }

        [Test]
        public void Command_PatchContainsNoDetails_FieldsAreNotSet()
        {
            var ukprn = 10000001;
            var larsCode = 1;
            var patchCommand = new JsonPatchDocument<PatchProviderCourse>();

            var command = new PatchProviderCourseCommand
            {
                Ukprn = ukprn,
                LarsCode = larsCode,
                Patch = patchCommand
            };

            command.IsApprovedByRegulator.Should().BeNull();
            command.IsPresentIsApprovedByRegulator.Should().BeFalse();
            command.ContactUsEmail.Should().BeNull();
            command.IsPresentContactUsEmail.Should().BeFalse();
            command.ContactUsPhoneNumber.Should().BeNull();
            command.IsPresentContactUsPhoneNumber.Should().BeFalse();
            command.StandardInfoUrl.Should().BeNull();
            command.IsPresentStandardInfoUrl.Should().BeFalse();
        }
    }
}
