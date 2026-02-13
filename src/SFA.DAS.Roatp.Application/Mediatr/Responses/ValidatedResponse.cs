using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace SFA.DAS.Roatp.Application.Mediatr.Responses
{
    public class ValidatedResponse { }

    public class ValidatedResponse<T> : ValidatedResponse
    {
        public T Result { get; }

        public IEnumerable<ValidationFailure> Errors { get; private set; } = [];

        public bool IsValidResponse => !Errors.Any();

        public ValidatedResponse(T model) => Result = model;

        public ValidatedResponse(IEnumerable<ValidationFailure> validationErrors) => Errors = validationErrors;
    }
}
