using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvvmCross.Platform;
using Nemeio.Core.Errors;

namespace Nemeio.Api.Controllers
{
    /// <summary>
    /// WARNING! This controller is only available for developer tests. It will be disable on release.
    /// </summary>
    [Route("api/dev/error")]
    [ApiController]
    public class ErrorController : DefaultController
    {
        private ILogger _logger;

        public ErrorController()
            : base()
        {
            _logger = Mvx.Resolve<ILoggerFactory>().CreateLogger<ErrorController>();
        }

        /// <summary>
        /// Developer purpose only ! Allow to test success and error response
        /// </summary>
        /// <param name="code">Error code</param>
        [HttpPost("{code}")]
        public IActionResult PostTestError(int code)
        {
            try
            {
                var apiErrorCode = (ErrorCode)code;
                if (apiErrorCode == ErrorCode.Success)
                {
                    return SuccessResponse();
                }
                else
                {
                    return ErrorResponse(apiErrorCode);
                }
            }
            catch (InvalidCastException exception)
            {
                _logger.LogError(exception, exception.Message);

                return BadRequest();
            }
        }
    }
}
