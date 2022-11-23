using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SFA.DAS.Roatp.Api.Infrastructure;

[ExcludeFromCodeCoverage]
public class AuthorizeByPathControllerModelConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        var controllerPath = controller.ControllerType.Namespace.Split('.').Last();
        controller.Filters.Add(controllerPath.Equals("ExternalReadControllers") ? new AuthorizeFilter(Constants.EndpointGroups.Integration) : new AuthorizeFilter(Constants.EndpointGroups.Management));
    }
}
