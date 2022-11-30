using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SFA.DAS.Roatp.Api.Infrastructure;

[ExcludeFromCodeCoverage]
public class ApiExplorerGroupingConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        var controllerPath = controller.ControllerType.Namespace.Split('.').Last();
        controller.ApiExplorer.GroupName = controllerPath.Equals("ExternalReadControllers") ? Constants.EndpointGroups.Integration : Constants.EndpointGroups.Management;
    }
}
