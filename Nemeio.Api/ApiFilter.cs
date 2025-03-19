using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using MvvmCross.Platform;
using Nemeio.Api.Dto.Out;
using Nemeio.Api.Exceptions;
using Nemeio.Core.Errors;

namespace Nemeio.Api
{
    public class ApiFilterAttribute : Attribute, IExceptionFilter
    {
        private readonly ILogger _logger;

        public ApiFilterAttribute()
        {
            var loggerFactory = Mvx.Resolve<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<ApiFilterAttribute>();
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "Api error");

            switch (context.Exception)
            {
                case ApiException exception:

                    var errorManager = Mvx.Resolve<IErrorManager>();
                    var errorDescription = errorManager.GetErrorDescription(exception.ErrorCode);

                    var result = new ObjectResult(new ErrorOutDto<BaseOutDto>()
                    {
                        ErrorCode = exception.ErrorCode,
                        ErrorDescription = errorDescription,
                        Result = null
                    });
                    
                    result.StatusCode = (int)HttpStatusCode.InternalServerError;

                    context.Result = result;
                    context.ExceptionHandled = true;

                    break;
            }
        }
    }
}
