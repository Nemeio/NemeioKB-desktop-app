using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nemeio.WinAutoInstaller.Models
{
    public class Logger
    {
        private const string LogSource                                  = "Application";

        private static Logger _instance                                 = null;
        private static readonly object _singletonInstanceLock           = new object();

        private EventLog _eventLog;
        private ErrorMessageProvider _errorMessageProvider;

        private Logger()
        {
            _eventLog = new EventLog(LogSource);
            _eventLog.Source = LogSource;
            _errorMessageProvider = new ErrorMessageProvider();
        }

        public static Logger Instance
        {
            get
            {
                lock (_singletonInstanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = new Logger();
                    }

                    return _instance;
                }
            }
        }

        public void LogInformation(string content) => Log(content, EventLogEntryType.Information);

        public void LogWarning(string content) => Log(content, EventLogEntryType.Warning);

        public void LogErrorCode(ErrorCode errorCode)
        {
            var errorMessage = _errorMessageProvider.GetFullErrorMessage(errorCode);

            Log(errorMessage, EventLogEntryType.Error);
        }

        public void LogException(Exception exception, string message = "")
        {
            var exceptionMessage    = exception?.Message ?? "";
            var exceptionStack      = exception?.StackTrace ?? "";

            var content = $"{exceptionMessage}\n{exceptionStack}\n\n{message}";

            Log(content, EventLogEntryType.Error);
        }

        private void Log(string content, EventLogEntryType type) => _eventLog?.WriteEntry(content, type);
    }
}
