using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvvmCross.Platform;
using Nemeio.Api.Dto.Out;
using Nemeio.Core.Managers;
using Nemeio.Core.Services;

namespace Nemeio.Api.Controllers
{
    [Route("api/about")]
    [ApiController]
    public class AboutController : DefaultController
    {
        private readonly ILogger _logger;
        private readonly IInformationService _informationService;
        private readonly ILanguageManager _languageManager;

        public AboutController()
            : base()
        {
            _logger = Mvx.Resolve<ILoggerFactory>().CreateLogger<AboutController>();
            _informationService = Mvx.Resolve<IInformationService>();
            _languageManager = Mvx.Resolve<ILanguageManager>();
        }

        /// <summary>
        /// Returns the current version of the landed application. The returned version has 4 digits (e.g. 0.1.30.5)
        /// </summary>
        [HttpGet("version")]
        public IActionResult GetApplicationVersion()
        {
            var outDto = new VersionOutDto()
            {
                Version = _informationService.GetApplicationVersion().ToString()
            };

            return SuccessResponse(outDto);
        }

        /// <summary>
        /// Returns the current platform of the landed application (e.g. x64).
        /// </summary>
        [HttpGet("architecture")]
        public IActionResult GetApplicationArchitecture()
        {
            var outDto = new ArchitectureOutDto()
            {
                Architecture = _informationService.GetApplicationArchitecture().ToString()
            };

            return SuccessResponse(outDto);
        }

        /// <summary>
        /// Returns all available fonts
        /// </summary>
        /// <returns>FontsOutDto</returns>
        [HttpGet("fonts")]
        public IActionResult GetFonts()
        {
            var outDto = new FontsOutDto()
            {
                Fonts = _informationService.GetSystemFonts()
            };

            return SuccessResponse(outDto);
        }

        /// <summary>
        /// Returns all available languages
        /// </summary>
        /// <returns>LanguageOutDto</returns>
        [HttpGet("languages")]
        public IActionResult GetSupportedLanguages()
        {
            var languagesOutDto = _languageManager.GetSupportedLanguages()
                                                    .Select(culture => LanguageOutDto.FromCulture(culture));

            return SuccessResponse(languagesOutDto);
        }
    }
}
