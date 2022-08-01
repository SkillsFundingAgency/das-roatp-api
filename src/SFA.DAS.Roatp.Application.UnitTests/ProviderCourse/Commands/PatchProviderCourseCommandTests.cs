using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Commands
{
    [TestFixture]
    public class PatchProviderCourseCommandTests
    {
        private const string Replace = "Replace";
        private const string ContactUsEmail = "ContactUsEmail";
        private const string ContactUsPhoneNumber = "ContactUsPhoneNumber";
        private const string ContactUsPageUrl = "ContactUsPageUrl";
        private const string StandardInfoUrl = "StandardInfoUrl";
        private const string IsApprovedByRegulator = "IsApprovedByRegulator";

        [Test]
        public void Command_PatchContainsStandardInfoUrl_StandardInfoUrlIsSet()
        {
            var ukprn = 10000001;
            var larsCode = 1;
            var testValue = "value";
            var patchCommand = new JsonPatchDocument<PatchProviderCourse>();
            patchCommand.Operations.Add(new Operation<PatchProviderCourse> {op=Replace,path=StandardInfoUrl,value=testValue});
           
            var command = new PatchProviderCourseCommand
            {
                Ukprn = ukprn,
                LarsCode = larsCode,
                Patch = patchCommand
            };

            Assert.AreEqual(testValue,command.StandardInfoUrl);
            Assert.IsTrue(command.IsPresentStandardInfoUrl);
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

            Assert.AreEqual(testValue, command.ContactUsEmail);
            Assert.IsTrue(command.IsPresentContactUsEmail);
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

            Assert.AreEqual(testValue, command.ContactUsPhoneNumber);
            Assert.IsTrue(command.IsPresentContactUsPhoneNumber);
        }

        [Test]
        public void Command_PatchContainsContactUsPageUrl_ContactUsPageUrlIsSet()
        {
            var ukprn = 10000001;
            var larsCode = 1;
            var testValue = "value";
            var patchCommand = new JsonPatchDocument<PatchProviderCourse>();
            patchCommand.Operations.Add(new Operation<PatchProviderCourse> { op = Replace, path = ContactUsPageUrl, value = testValue });

            var command = new PatchProviderCourseCommand
            {
                Ukprn = ukprn,
                LarsCode = larsCode,
                Patch = patchCommand
            };

            Assert.AreEqual(testValue, command.ContactUsPageUrl);
            Assert.IsTrue(command.IsPresentContactUsPageUrl);
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

            Assert.AreEqual(true, command.IsApprovedByRegulator);
            Assert.IsTrue(command.IsPresentIsApprovedByRegulator);
        }
    }
}
