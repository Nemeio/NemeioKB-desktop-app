using System;
using System.Text;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Errors;
using Nemeio.Core.Services;
using Security;

namespace Nemeio.Platform.Mac.Applications
{
    public class MacProtectedDataProvider : IProtectedDataProvider
    {
        private const string ServiceName = "Nemeio";

        private readonly ILogger _logger;
        private readonly IErrorManager _errorManager;

        public MacProtectedDataProvider(ILoggerFactory loggerFactory, IErrorManager errorManager)
        {
            _logger = loggerFactory.CreateLogger<MacProtectedDataProvider>();
            _errorManager = errorManager;
        }

        public string GetPassword(string storeName)
        {
            byte[] password;

            var code = SecKeyChain.FindGenericPassword(
                ServiceName, 
                storeName, 
                out password
            );

            _logger.LogInformation($"Keychain result code <{code}>");

            if (code != SecStatusCode.Success)
            {
                _logger.LogWarning($"GetPassword : Can't access to Keychain. {_errorManager.GetFullErrorMessage(ErrorCode.MacReadKeyFromKeychainFailed)}");

                throw new UnauthorizedAccessException("Can't access to Keychain");
            }

            if (password == null)
            {
                throw new InvalidOperationException("Password is null");
            }

            return Encoding.UTF8.GetString(password);
        }

        public void SavePassword(string storeName, string password)
        {
            var code = SecKeyChain.AddGenericPassword(
                ServiceName,
                storeName,
                Encoding.UTF8.GetBytes(password)
            );

            if (code == SecStatusCode.DuplicateItem)
            {
                //  Password already exists

                throw new ArgumentException($"Key exists already. Keychain code : <{code}>");
            }

            if (code != SecStatusCode.Success)
            {
                _logger.LogWarning($"SavePassword : Can't save password on Keychain <{code}> : {_errorManager.GetFullErrorMessage(ErrorCode.MacWriteKeyFromKeychainFailed)}");

                throw new UnauthorizedAccessException($"Can't save password on Keychain <{code}>");
            }
        }
    }
}
