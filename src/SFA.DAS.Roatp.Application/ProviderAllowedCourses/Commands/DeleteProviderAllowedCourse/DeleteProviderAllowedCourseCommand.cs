using MediatR;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.DeleteProviderAllowedCourse;

public class DeleteProviderAllowedCourseCommand : IRequest
{
    public int Ukprn { get; set; }
    public string LarsCode { get; set; }
    public string UserId { get; set; }
    public string UserDisplayName { get; set; }
}
