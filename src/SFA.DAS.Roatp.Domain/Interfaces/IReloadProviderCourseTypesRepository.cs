using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces;

public interface IReloadProviderCourseTypesRepository
{
    Task<bool> ReloadProviderCourseTypes(List<ProviderCourseType> providerCourseTypes, DateTime timeStarted);
}