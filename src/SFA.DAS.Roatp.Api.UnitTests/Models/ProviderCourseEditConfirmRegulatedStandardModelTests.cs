using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.UpdateApprovedByRegulator;

namespace SFA.DAS.Roatp.Api.UnitTests.Models
{
    [TestFixture]
    class ProviderCourseEditConfirmRegulatedStandardModelTests
    {
        [Test, AutoData]
        public void ImplicitOperator_ReturnsCommand(ProviderCourseEditConfirmRegulatedStandardModel model)
        {
            ((UpdateApprovedByRegulatorCommand)model).Should().BeEquivalentTo(model);
        }
    }
}
