﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Jobs.Services.CourseDirectory
{
    public interface IGetBetaProvidersService
    {
        Task<List<int>> GetBetaProviderUkprns();
    }
}