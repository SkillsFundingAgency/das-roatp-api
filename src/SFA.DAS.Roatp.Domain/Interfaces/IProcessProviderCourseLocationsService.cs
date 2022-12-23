using System.Collections.Generic;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.Interfaces;

public interface IProcessProviderCourseLocationsService
{
    List<DeliveryModel> ConvertProviderLocationsToDeliveryModels(List<ProviderCourseLocationDetailsModel> providerCourseLocations);
    List<DeliveryModelWithAddress> ConvertProviderLocationsToDeliveryModelWithAddress(List<ProviderCourseLocationDetailsModel> providerCourseLocations);

}
