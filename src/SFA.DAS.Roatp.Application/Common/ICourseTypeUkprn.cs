using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Common
{
    public interface ICourseTypeUkprn
    {
        int Ukprn { get; }
        public CourseType CourseType { get; }
    }
}
