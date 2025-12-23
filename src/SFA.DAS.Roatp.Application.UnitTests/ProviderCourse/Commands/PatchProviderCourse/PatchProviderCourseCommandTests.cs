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

        [Test]
        public void Command_PatchContainsStandardInfoUrlWithUppercaseReplace_StandardInfoUrlIsSet()
        {
            var ukprn = 10000002;
            var larsCode = "2";
            var testValue = "upper-case-replace";
            var patchCommand = new JsonPatchDocument<Domain.Models.PatchProviderCourse>();
            patchCommand.Operations.Add(new Operation<Domain.Models.PatchProviderCourse> { op = "REPLACE", path = StandardInfoUrl, value = testValue });

            var command = new PatchProviderCourseCommand
            {
                Ukprn = ukprn,
                LarsCode = larsCode,
                Patch = patchCommand
            };

            command.StandardInfoUrl.Should().Be(testValue);
            command.IsPresentStandardInfoUrl.Should().BeTrue();
        }

        [Test]
        public void Command_PatchContainsStandardInfoUrlWithNonReplaceOp_Ignored()
        {
            var ukprn = 10000003;
            var larsCode = "3";
            var patchCommand = new JsonPatchDocument<Domain.Models.PatchProviderCourse>();
            patchCommand.Operations.Add(new Operation<Domain.Models.PatchProviderCourse> { op = "add", path = StandardInfoUrl, value = "ignored" });

            var command = new PatchProviderCourseCommand
            {
                Ukprn = ukprn,
                LarsCode = larsCode,
                Patch = patchCommand
            };

            command.StandardInfoUrl.Should().BeNull();
            command.IsPresentStandardInfoUrl.Should().BeFalse();
        }

        [Test]
        public void Command_PatchContainsIsApprovedByRegulatorWithInvalidValue_IsApprovedByRegulatorIsNullButPresent()
        {
            var ukprn = 10000004;
            var larsCode = "4";
            var invalidValue = "not-a-bool";
            var patchCommand = new JsonPatchDocument<Domain.Models.PatchProviderCourse>();
            patchCommand.Operations.Add(new Operation<Domain.Models.PatchProviderCourse> { op = Replace, path = IsApprovedByRegulator, value = invalidValue });

            var command = new PatchProviderCourseCommand
            {
                Ukprn = ukprn,
                LarsCode = larsCode,
                Patch = patchCommand
            };

            command.IsApprovedByRegulator.Should().BeNull();
            command.IsPresentIsApprovedByRegulator.Should().BeTrue();
        }

        [Test]
        public void Command_PatchContainsHasOnlineDeliveryOptionWithInvalidValue_HasOnlineDeliveryOptionIsNullButPresent()
        {
            var ukprn = 10000005;
            var larsCode = "5";
            var invalidValue = "123";
            var patchCommand = new JsonPatchDocument<Domain.Models.PatchProviderCourse>();
            patchCommand.Operations.Add(new Operation<Domain.Models.PatchProviderCourse> { op = Replace, path = HasOnlineDeliveryOption, value = invalidValue });

            var command = new PatchProviderCourseCommand
            {
                Ukprn = ukprn,
                LarsCode = larsCode,
                Patch = patchCommand
            };

            command.HasOnlineDeliveryOption.Should().BeNull();
            command.IsPresentHasOnlineDeliveryOption.Should().BeTrue();
        }
    }
}