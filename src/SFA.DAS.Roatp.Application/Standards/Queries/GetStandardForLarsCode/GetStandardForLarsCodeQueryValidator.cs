using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Standards.Queries.GetStandardForLarsCode
{
    public class GetStandardForLarsCodeQueryValidator : AbstractValidator<GetStandardForLarsCodeQuery>
    {
        public GetStandardForLarsCodeQueryValidator(IStandardsReadRepository standardsReadRepository)
        {
            Include(new LarsCodeValidator(standardsReadRepository));
        }
    }
}
