using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.UpdateProviderCourse;

namespace SFA.DAS.Roatp.Api.UnitTests.Models
{
    [TestFixture]
    public class ProviderCourseEditModelTests
    {
        [Test, AutoData]
        public void ImplicitOperator_ReturnsCommand(ProviderCourseEditModel model)
        {
            ((UpdateProviderCourseCommand)model).Should().BeEquivalentTo(model);
        }
    }
}
