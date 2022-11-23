using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Linq;

namespace SFA.DAS.Roatp.Api.Infrastructure
{
    public class ApiExplorerGroupingConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var controllerPath = controller.ControllerType.Namespace.Split('.').Last();
            controller.ApiExplorer.GroupName = controllerPath.Equals("ExternalReadControllers") ? Constants.EndpointGroups.Integration : Constants.EndpointGroups.Operation;
        }
    }
}
