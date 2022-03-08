﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Api.Models;

namespace SFA.DAS.Roatp.Api.Services
{
    public interface IGetProviderCoursesService
    {
        Task<List<ProviderCourseModel>> GetAll(int ukprn);
        Task<ProviderCourseModel> Get(int ukprn, int larsCode);
    }
}