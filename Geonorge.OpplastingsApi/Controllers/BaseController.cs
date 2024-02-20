using Geonorge.OpplastingsApi.Exceptions;
using Geonorge.OpplastingsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Geonorge.OpplastingsApi.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        private readonly ILogger<ControllerBase> _logger;

        protected BaseController(
            ILogger<ControllerBase> logger)
        {
            _logger = logger;
        }

        protected IActionResult HandleException(Exception exception)
        {
            _logger.LogError(exception.ToString());

            return exception switch
            {
                ArgumentException _ or FormatException _ => BadRequest(),
                FileValidationException ex => BadRequest(ex.Message),
                UnauthorizedAccessException ex => StatusCode(StatusCodes.Status401Unauthorized, ex.Message),
                AuthorizationException ex => StatusCode(StatusCodes.Status403Forbidden, ex.Message),
                Exception _ => StatusCode(StatusCodes.Status500InternalServerError),
                _ => null,
            };
        }
    }
}
