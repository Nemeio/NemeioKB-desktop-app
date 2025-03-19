using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvvmCross.Platform;
using Nemeio.Api.Dto.In;
using Nemeio.Api.Dto.Out;
using Nemeio.Core.Applications;
using Nemeio.Core.Errors;
using Nemeio.Core.Layouts;
using Nemeio.Core.Managers;

namespace Nemeio.Api.Controllers
{
    [Route("api/settings")]
    [ApiController]
    public class ApplicationSettingsController : DefaultController
    {
        public readonly ILogger _logger;
        public readonly ILanguageManager _languageManager;
        public readonly IApplicationSettingsProvider _applicationSettingsManager;
        public readonly ILayoutFacade _layoutFacade;

        public ApplicationSettingsController()
            : base()
        {
            _logger = Mvx.Resolve<ILoggerFactory>().CreateLogger<CategoryController>();
            _languageManager = Mvx.Resolve<ILanguageManager>();
            _applicationSettingsManager = Mvx.Resolve<IApplicationSettingsProvider>();
            _layoutFacade = Mvx.Resolve<ILayoutFacade>();
        }

        /// <summary>
        /// Allows you to retrieve the language used to display information about the desktop application. By default, the language is the system language, until the user manually changes it via the configurator.
        /// </summary>
        [HttpGet("language")]
        public IActionResult GetApplicationLanguage()
        {
            var currentCultureInfo = _languageManager.GetCurrentCultureInfo();
            var outDto = LanguageOutDto.FromCulture(currentCultureInfo);

            return SuccessResponse(outDto);
        }

        /// <summary>
        /// Allows you to change the current language
        /// </summary>
        /// <param name="inDto">Wanted language</param>
        [HttpPost("language")]
        public IActionResult SetApplicationLanguage([FromBody] LanguageInDto inDto)
        {
            var selectedLanguage = new CultureInfo(inDto.Language);
            var contains = _languageManager
                .GetSupportedLanguages()
                .Select(x => x.TwoLetterISOLanguageName)
                .Contains(selectedLanguage.TwoLetterISOLanguageName);

            if (!contains)
            {
                return ErrorResponse(ErrorCode.ApiInvalidParameters);
            }

            _languageManager.SetCurrentCultureInfo(selectedLanguage);

            return SuccessResponse();
        }

        /// <summary>
        /// Allow to retrieve application's settings. Contains current language, augmented image status (enabled / disabled), grant privilege window status (enabled / disabled).
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetApplicationSettings()
        {
            var settings = _applicationSettingsManager.ApplicationSettings;
            var outDto = ApplicationSettingsOutDto.FromModel(settings);

            return SuccessResponse(outDto);
        }

        /// <summary>
        /// Allow user to change augmented image status : enabled or disabled
        /// </summary>
        /// <param name="inDto">Wanted status</param>
        /// <returns>ApplicationSettingsOutDto</returns>
        [HttpPost("augmented")]
        public async Task<IActionResult> SetAugmentedImageStatus([FromBody] AugmentedImageStatusInDto inDto)
        {
            _applicationSettingsManager.AugmentedImageEnable = inDto.Status;

            await _layoutFacade.RefreshAugmentedLayoutAsync();

            var outDto = ApplicationSettingsOutDto.FromModel(_applicationSettingsManager.ApplicationSettings);

            return SuccessResponse(outDto);
        }

        /// <summary>
        /// Allow user to change grant privilege window status : enabled or disabled
        /// </summary>
        /// <returns>ApplicationSettingsOutDto</returns>
        /// <param name="showGrantPrivilegeWindow">Wanted status</param>
        [HttpPost("showGrantPrivilegeWindow")]
        public IActionResult SetShowGrantPrivilegeWindow([FromBody] ShowGrantPrivilegeWindow inDto)
        {
            _applicationSettingsManager.ShowGrantPrivilegeWindow = inDto.Status;

            var outDto = ApplicationSettingsOutDto.FromModel(_applicationSettingsManager.ApplicationSettings);

            return SuccessResponse(outDto);
        }
    }
}
