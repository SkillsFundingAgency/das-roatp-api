﻿using MediatR;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries
{
    public class ProviderCourseQuery : IRequest<ProviderCourseQueryResult>, ILarsCode, IUkprn
    {
        public int Ukprn { get; }
        public int LarsCode { get; }

        public ProviderCourseQuery(int ukprn, int larsCode)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
        }
    }
}