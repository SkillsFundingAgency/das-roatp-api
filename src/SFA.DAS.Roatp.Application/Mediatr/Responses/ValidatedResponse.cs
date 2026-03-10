using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace SFA.DAS.Roatp.Application.Mediatr.Responses;

public class ValidatedResponse
{
    public IEnumerable<ValidationFailure> Errors { get; protected set; }
    public bool IsValidResponse => !Errors.Any();
    protected ValidatedResponse() => Errors = [];
    public ValidatedResponse(IEnumerable<ValidationFailure> validationErrors) => Errors = validationErrors;
    public static ValidatedResponse Valid() => new();
}

public class ValidatedResponse<T> : ValidatedResponse
{
    public T Result { get; }

    public ValidatedResponse(T model) => Result = model;

    public ValidatedResponse(IEnumerable<ValidationFailure> validationFailures) : base(validationFailures)
    { }
}
