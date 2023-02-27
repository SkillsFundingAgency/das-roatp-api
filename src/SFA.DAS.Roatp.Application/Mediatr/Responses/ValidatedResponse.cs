using FluentValidation.Results;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SFA.DAS.Roatp.Application.Mediatr.Responses
{
    public class ValidatedResponse { }
    
    public class ValidatedResponse<T> : ValidatedResponse
    {
         public T Result { get; }
        private readonly IList<ValidationFailure> _errorMessages = new List<ValidationFailure>();

        public IReadOnlyCollection<ValidationFailure> Errors => new ReadOnlyCollection<ValidationFailure>(_errorMessages);
        public bool IsValidResponse => !_errorMessages.Any();

        public ValidatedResponse(T model) => Result = model;
        public ValidatedResponse(IList<ValidationFailure> validationErrors) => _errorMessages = validationErrors;
     
    }
}
