﻿using SFA.DAS.Roatp.Domain.Entities;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderCourseEditRepository
    {
        Task UpdateContactDetails(ProviderCourse updatedProviderCourseEntity);
        Task UpdateConfirmRegulatedStandard(ProviderCourse updatedProviderCourseEntity);
    }
}
