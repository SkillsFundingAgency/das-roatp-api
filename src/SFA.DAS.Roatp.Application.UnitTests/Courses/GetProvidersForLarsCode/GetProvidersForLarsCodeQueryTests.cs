using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode.V1;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetProvidersForLarsCode;

[TestFixture]
public class GetProvidersForLarsCodeQueryTests
{
    [Test, RecursiveMoqAutoData]
    public void Operator_PopulatesQueryFromGetProvidersFromLarsCodeRequest(string larsCode, GetProvidersFromLarsCodeRequest request)
    {
        var model = new GetProvidersForLarsCodeQuery(larsCode, request);
        var mappedDeliveryModes = new List<DeliveryModeV1>();
        mappedDeliveryModes.AddRange(from val in request.DeliveryModes where val != null select (DeliveryModeV1)val);

        var mappedEmployerProviderRatings = new List<ProviderRating>();
        mappedEmployerProviderRatings.AddRange(from val in request.EmployerProviderRatings where val != null select (ProviderRating)val);

        var mappedApprenticeProviderRatings = new List<ProviderRating>();
        mappedApprenticeProviderRatings.AddRange(from val in request.ApprenticeProviderRatings where val != null select (ProviderRating)val);

        var mappedQarRatings = new List<QarRating>();
        mappedQarRatings.AddRange(from val in request.Qar where val != null select (QarRating)val);

        using (new AssertionScope())
        {
            model.Should().NotBeNull();

            model.LarsCode.Should().Be(larsCode);
            model.Latitude.Should().Be(request.Latitude);
            model.Longitude.Should().Be(request.Longitude);
            model.OrderBy.Should().Be(request.OrderBy);
            model.Distance.Should().Be(request.Distance);
            model.Page.Should().Be(request.Page);
            model.PageSize.Should().Be(request.PageSize);

            model.DeliveryModes.Should().BeEquivalentTo(mappedDeliveryModes);
            model.EmployerProviderRatings.Should().BeEquivalentTo(mappedEmployerProviderRatings);
            model.ApprenticeProviderRatings.Should().BeEquivalentTo(mappedApprenticeProviderRatings);
            model.Qar.Should().BeEquivalentTo(mappedQarRatings);
        }
    }
}
