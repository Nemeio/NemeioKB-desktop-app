using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Errors;
using Nemeio.Core.Services;

namespace Nemeio.Core.Keys.Executors
{
    public class UrlKeyExecutor : KeyExecutor
    {
        private readonly ILogger _logger;
        private readonly IBrowserFile _browserFile;
        private readonly IDialogService _dialogService;
        private readonly IErrorManager _errorManager;

        public UrlKeyExecutor(ILoggerFactory loggerFactory, IBrowserFile browserFile, IDialogService dialogService, IErrorManager errorManager, string data)
            : base(data)
        {
            _logger = loggerFactory.CreateLogger<UrlKeyExecutor>();
            _browserFile = browserFile ?? throw new ArgumentNullException(nameof(browserFile));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _errorManager = errorManager ?? throw new ArgumentNullException(nameof(errorManager));
        }

        public override async Task ExecuteAsync()
        {
            await Task.Yield();

            try
            {
                if (Data == string.Empty)
                {
                    throw new InvalidOperationException(nameof(Data));
                }

                _browserFile.OpenUrl(Data);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    _errorManager.GetFullErrorMessage(ErrorCode.CoreOpenUrlFailed)
                );

                _dialogService.ShowMessageAsync(
                    StringId.ApplicationTitleName,
                    StringId.ApplicationErrorOpenWebsite
                );
            }
        }
    }
}
