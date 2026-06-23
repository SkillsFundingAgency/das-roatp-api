using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.RestrictedCourses.Commands.AddRestrictedCourse;

public class AddRestrictedCourseCommandHandler : IRequestHandler<AddRestrictedCourseCommand, ValidatedResponse<int>>
{
    public Task<ValidatedResponse<int>> Handle(AddRestrictedCourseCommand request, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}
