using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace SFA.DAS.Roatp.Api.Infrastructure
{
    public class ActionResponseControllerBase : ControllerBase
    {
        protected IActionResult GetResponse<T>(ValidatedResponse<T> response) where T : class
        {
            if (response.Result == null && response.IsValidResponse) return NotFound();

            if (response.IsValidResponse) return new OkObjectResult(response.Result);

            return new BadRequestObjectResult(FormatErrors(response.Errors));
        }

        protected IActionResult GetNoContentResponse<T>(ValidatedResponse<T> response)
        {
            if (response.Result == null && response.IsValidResponse) return NotFound();

            if (response.IsValidResponse) return new NoContentResult();

            return new BadRequestObjectResult(FormatErrors(response.Errors));
        }

        protected IActionResult GetPostResponse<T>(ValidatedResponse<T> response, string uri) 
        {
            if (response.IsValidResponse)
                return new CreatedResult(uri,response.Result);

            return new BadRequestObjectResult(FormatErrors(response.Errors));
        }

        private static List<ValidationError> FormatErrors(IEnumerable<ValidationFailure> errors)
        {
            return errors.Select(err => new ValidationError
            {
                PropertyName = err.PropertyName,
                ErrorMessage = err.ErrorMessage
            }).ToList();
        }
    }
}
