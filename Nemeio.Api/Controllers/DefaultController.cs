using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvvmCross.Platform;
using Nemeio.Api.Dto.Out;
using Nemeio.Core.Errors;

namespace Nemeio.Api.Controllers
{
    public class DefaultController: ControllerBase
    {
        private const int HttpForbiddenCode = 403;
        private IErrorManager _errorManager;

        public DefaultController() => _errorManager = Mvx.Resolve<IErrorManager>();

        protected IActionResult Forbidden()
        {
            return StatusCode(HttpForbiddenCode);
        }

        protected IActionResult SuccessResponse() => SuccessResponse<BaseOutDto>(null);

        protected IActionResult SuccessResponse<T>(T outDto)
        {
            var errorCode = ErrorCode.Success;
            var errorDescription = _errorManager.GetErrorDescription(errorCode);

            return Ok(
                new ErrorOutDto<T>()
                {
                    ErrorCode = errorCode,
                    ErrorDescription = errorDescription,
                    Result = outDto
                }    
            );
        }

        /// <summary>
        /// We always return a 200 HttpCode because webserver communication is valid but 
        /// actully there are an error occcured on Api call
        /// </summary>
        protected IActionResult ErrorResponse(ErrorCode errorCode)
        {
            var errorDescription = _errorManager.GetErrorDescription(errorCode);

            return StatusCode(StatusCodes.Status500InternalServerError, 
                new ErrorOutDto<BaseOutDto>()
                {
                    ErrorCode = errorCode,
                    ErrorDescription = errorDescription,
                    Result = null
                }
            );
        }
    }
}
