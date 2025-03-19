using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvvmCross.Platform;
using Nemeio.Api.Dto.In;

namespace Nemeio.Api.Controllers
{
    [Route("api/logs")]
    [ApiController]
    public class LogsController : DefaultController
    {
        private ILogger _logger;

        public LogsController()
            : base()
        {
            _logger = Mvx.Resolve<ILoggerFactory>().CreateLogger<LogsController>();
        }

        /// <summary>
        /// Allow developer to post log on desktop application's logs
        /// </summary>
        /// <param name="log">Log content</param>
        [HttpPost]
        public IActionResult PostErrorLogs([FromBody] LogInDto log)
        {
            _logger.LogError($"[CONFIGURATOR] {log.Content}");

            return SuccessResponse();
        }
    }
}
