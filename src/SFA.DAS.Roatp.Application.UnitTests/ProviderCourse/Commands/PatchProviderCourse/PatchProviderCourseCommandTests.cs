using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.PatchProviderCourse;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Commands.PatchProviderCourse
{
    [TestFixture]
    public class PatchProviderCourseCommandTests
    {
        private const string Replace = "replace";
        private const string ContactUsEmail = "ContactUsEmail";
        private const string ContactUsPhoneNumber = "ContactUsPhoneNumber";
        private const string StandardInfoUrl = "StandardInfoUrl";
        private const string IsApprovedByRegulator = "IsApprovedByRegulator";
        private const string HasOnlineDeliveryOption = "HasOnlineDeliveryOption";

        [Test]
        public void Command_PatchContainsStandardInfoUrl_StandardInfoUrlIsSet()
        {
            var ukprn = 10000001;
            var larsCode = "1";
            var testValue = "value";
            var patchCommand = new JsonPatchDocument<Domain.Models.PatchProviderCourse>();
            patchCommand.Operations.Add(new Operation<Domain.Models.PatchProviderCourse> { op = Replace, path = StandardInfoUrl, value = testValue });

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
            var larsCode = "1";
            var testValue = "value";
            var patchCommand = new JsonPatchDocument<Domain.Models.PatchProviderCourse>();
            patchCommand.Operations.Add(new Operation<Domain.Models.PatchProviderCourse> { op = Replace, path = ContactUsEmail, value = testValue });

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
            var larsCode = "1";
            var testValue = "value";
            var patchCommand = new JsonPatchDocument<Domain.Models.PatchProviderCourse>();
            patchCommand.Operations.Add(new Operation<Domain.Models.PatchProviderCourse> { op = Replace, path = ContactUsPhoneNumber, value = testValue });

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
            var larsCode = "1";
            var testValue = "true";
            var patchCommand = new JsonPatchDocument<Domain.Models.PatchProviderCourse>();
            patchCommand.Operations.Add(new Operation<Domain.Models.PatchProviderCourse> { op = Replace, path = IsApprovedByRegulator, value = testValue });

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
        public void Command_PatchContainsHasOnlineDeliveryOption_HasOnlineDeliveryOptionIsSet()
        {
            var ukprn = 10000001;
            var larsCode = "1";
            var testValue = "true";
            var patchCommand = new JsonPatchDocument<Domain.Models.PatchProviderCourse>();
            patchCommand.Operations.Add(new Operation<Domain.Models.PatchProviderCourse> { op = Replace, path = HasOnlineDeliveryOption, value = testValue });

            var command = new PatchProviderCourseCommand
            {
                Ukprn = ukprn,
                LarsCode = larsCode,
                Patch = patchCommand
            };

            command.HasOnlineDeliveryOption.Should().BeTrue();
            command.IsPresentHasOnlineDeliveryOption.Should().BeTrue();
        }

        [Test]
        public void Command_PatchContainsNoDetails_FieldsAreNotSet()
        {
            var ukprn = 10000001;
            var larsCode = "1";
            var patchCommand = new JsonPatchDocument<Domain.Models.PatchProviderCourse>();

            var command = new PatchProviderCourseCommand
            {
                Ukprn = ukprn,
                LarsCode = larsCode,
                Patch = patchCommand
            };

            command.IsApprovedByRegulator.Should().BeNull();
            command.IsPresentHasOnlineDeliveryOption.Should().BeFalse();
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