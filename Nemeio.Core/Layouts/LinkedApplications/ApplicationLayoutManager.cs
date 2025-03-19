using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Errors;
using Nemeio.Core.Services.Blacklist;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems.Applications;

namespace Nemeio.Core.Layouts.LinkedApplications
{
    public class ApplicationLayoutManager : IApplicationLayoutManager
    {
        private readonly ILogger _logger;
        private readonly ILayoutLibrary _layoutLibrary;
        private readonly IBlacklistDbRepository _blacklistDbRepository;
        private readonly List<string> _exceptionNameList = new List<string>();
        private readonly IErrorManager _errorManager;

        public IList<string> ExceptionNameList { get => _exceptionNameList; }

        public ApplicationLayoutManager(ILoggerFactory loggerFactory, IBlacklistDbRepository blacklistDbRepository, ILayoutLibrary layoutLibrary, IErrorManager errorManager)
        {
            _logger = loggerFactory.CreateLogger<ApplicationLayoutManager>();
            _layoutLibrary = layoutLibrary;
            _errorManager = errorManager;

            _blacklistDbRepository = blacklistDbRepository;
            var systemBlacklist = _blacklistDbRepository.ReadSystemBlacklists();
            var blacklist = _blacklistDbRepository.ReadBlacklists();

            _exceptionNameList.AddRange(systemBlacklist.Select(item => item.Name.ToLower()));
            _exceptionNameList.AddRange(blacklist.Select(item => item.Name.ToLower()));
        }

        public LayoutId FindLatestAssociatedLayoutId(Application application)
        {
            // sanity check that path is defined
            if (string.IsNullOrWhiteSpace(application.ApplicationPath))
            {
                return null;
            }

            // build uriName (if any at all)
            var uriName = BuildVerifiedUriName(application.ApplicationPath);
            if (uriName == null)
            {
                return null;
            }

            var linkedApplication = GetLayoutByLinkedApplicationPath(application.ApplicationPath);
            if (linkedApplication != null)
            {
                _logger.LogInformation($"ApplicationLayoutManager.FindLatestAssociatedLayoutId: Associated layout found <{linkedApplication.LayoutId}> for path <{application.ApplicationPath}>");
                return linkedApplication.LayoutId;
            }

            return null;
        }

        public ILayout GetLayoutByLinkedApplicationPath(string applicationPathOrName)
        {
            var result = _layoutLibrary.Layouts.FirstOrDefault(x => MatchLinkedAppPath(x, applicationPathOrName));
            if (result == null)
            {
                var executable = Path.GetFileName(applicationPathOrName);
                result = _layoutLibrary.Layouts.FirstOrDefault(x => MatchLinkedAppPath(x, executable));
            }
            return result;
        }

        public string FindException(Application application)
        {
            string result = null;
            try
            {
                var applicationName = application.Name;
                if (applicationName != null)
                {
                    var checkName = applicationName.ToLower();
                    result = ExceptionNameList.Where(name => checkName.Contains(name)).FirstOrDefault();
                }
                else
                {
                    _logger.LogInformation("ApplicationLayoutManager.FindException Empty Application Name");
                }
            }
            catch (FileNotFoundException exception)
            {
                _logger.LogError(
                    exception, 
                    _errorManager.GetFullErrorMessage(ErrorCode.CoreFindExceptionFromBlacklistFailed)
                );
            }
            return result;
        }

        private bool MatchLinkedAppPath(ILayout layout, string applicationPath)
        {
            var currentLayoutPaths = layout.LayoutInfo.LinkApplicationPaths;

            if (currentLayoutPaths == null)
            {
                return false;
            }

            return currentLayoutPaths.Contains(applicationPath) && layout.LayoutInfo.LinkApplicationEnable;
        }

        private Uri BuildVerifiedUriName(string applicationPath)
        {
            var uriName = new Uri(applicationPath);
            var checkName = Uri.UnescapeDataString(applicationPath);

            if (uriName == null || (!File.Exists(checkName) && !Directory.Exists(checkName)))
            {
                return null;
            }

            return uriName;
        }
    }
}
