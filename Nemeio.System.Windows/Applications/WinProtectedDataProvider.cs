using System;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Errors;
using Nemeio.Core.Services;

namespace Nemeio.Platform.Windows.Applications
{
    public class WinProtectedDataProvider : IProtectedDataProvider
    {
        private readonly ILogger _logger;
        private readonly IErrorManager _errorManager;

        public WinProtectedDataProvider(ILoggerFactory loggerFactory, IErrorManager errorManager)
        {
            _logger = loggerFactory.CreateLogger<WinProtectedDataProvider>();
            _errorManager = errorManager;
        }

        public string GetPassword(string storeName)
        {
            try
            {
                using (IsolatedStorageFile isoStore = GetStore())
                using (var passwordStreamWriter = new StreamReader(isoStore.OpenFile(storeName, FileMode.Open)))
                {
                    return passwordStreamWriter.ReadToEnd();
                }
            }
            catch (Exception exception) when (exception is IsolatedStorageException ||
                                              exception is FileNotFoundException)
            {
                _logger.LogError(
                    exception, 
                    _errorManager.GetFullErrorMessage(ErrorCode.WindowsReadFileFromIsolatedStorageFailed)
                );

                throw new InvalidOperationException("Can't get password");
            }
        }

        public void SavePassword(string storeName, string password)
        {
            try
            {
                using (IsolatedStorageFile isoStore = GetStore())
                using (var passwordStreamWriter = new StreamWriter(isoStore.CreateFile(storeName)))
                {
                    passwordStreamWriter.Write(password);
                }
            }
            catch (IsolatedStorageException exception)
            {
                _logger.LogError(
                    exception,
                    _errorManager.GetFullErrorMessage(ErrorCode.WindowsWriteFileFromIsolatedStorageFailed)
                );

                throw new UnauthorizedAccessException("Can't save password on IsolatedStorage");
            }
        }

        private static IsolatedStorageFile GetStore() => IsolatedStorageFile.GetStore(GetScope(), null, null);

        public static IsolatedStorageScope GetScope() => IsolatedStorageScope.Assembly | IsolatedStorageScope.User;
    }
}
