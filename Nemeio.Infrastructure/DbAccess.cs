using System;
using System.Data.Common;
using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Nemeio.Core;
using Nemeio.Core.Errors;
using Nemeio.Core.Services;
using Nemeio.Infrastructure.Exceptions;
using Nemeio.Infrastructure.Extensions;

namespace Nemeio.Infrastructure
{
    public class DbAccess : IDisposable
    {
        private int _retryCount = 0;
        private int _retryMaxValue = 5;
        private readonly ILogger _logger;
        protected readonly IDocument _documentService;
        private readonly IProtectedDataProvider _protectedDataProvider;
        private readonly IErrorManager _errorManager;
        private DbConnection _dbConnection;

        private static string _databasePassword = null;
        public NemeioDbContext DbContext { get; }

        public DbAccess(ILoggerFactory loggerFactory, IProtectedDataProvider protectedDataProvider, IDocument documentService, IErrorManager errorManager)
        {
            _logger = loggerFactory.CreateLogger<DbAccess>();
            _logger.LogInformation($"DbAccess.Constructor()");

            _protectedDataProvider = protectedDataProvider;
            _documentService = documentService;
            _errorManager = errorManager;

            var dbFilePath = _documentService.DatabasePath;

            _dbConnection = Connect(dbFilePath, $"Data Source={dbFilePath};");
            DbContext = new NemeioDbContext(_dbConnection);
        }

        private string GetDatabasePassword(string dbFileName, string connectionString)
        {
            if (!string.IsNullOrEmpty(_databasePassword))
            {
                //  Database password has already been loaded

                return _databasePassword;
            }

            if (!File.Exists(dbFileName))
            {
                //  Database doesn't exists
                //  We create it and secure it.

                _logger.LogInformation(
                    _errorManager.GetFullErrorMessage(ErrorCode.DatabaseFileNotFound)
                );

                try
                {
                    _protectedDataProvider.SavePassword(
                        NemeioConstants.PasswordFilename,
                        new DbPasswordProvider().CreatePassword()
                    );
                }
                catch (ArgumentException exception)
                {
                    _logger.LogError(exception, "Key exists already, nothing to do.");
                }
                catch (UnauthorizedAccessException exception)
                {
                    _logger.LogError(
                        exception,
                        _errorManager.GetFullErrorMessage(ErrorCode.DatabaseSaveProtectedKeyFailed)
                    );

                    throw;
                }
            }

            try
            {
                var password = _protectedDataProvider.GetPassword(NemeioConstants.PasswordFilename);

                //  TODO: Must be remove on production
                _logger.LogInformation($"Password {password}");

                return password;
            }
            catch (UnauthorizedAccessException exception)
            {
                //  Only on OSX
                _logger.LogError(
                    exception,
                    _errorManager.GetFullErrorMessage(ErrorCode.DatabaseKeychainAccessDenied)
                );

                throw;
            }
            catch (InvalidOperationException exception)
            {
                _logger.LogError(
                    exception,
                    _errorManager.GetFullErrorMessage(ErrorCode.DatabaseRetrieveProtectedKeyFailed)
                );

                return RetryLoadDatabase(dbFileName, connectionString);
            }

        }

        private string RetryLoadDatabase(string dbFileName, string connectionString)
        {
            DeleteDatabaseFile(dbFileName);

            _retryCount += 1;

            if (_retryCount < _retryMaxValue)
            {
                _logger.LogInformation($"Retry database connection {_retryCount}/{_retryMaxValue}");

                return GetDatabasePassword(dbFileName, connectionString);
            }
            else
            {
                _logger.LogError(
                    _errorManager.GetFullErrorMessage(ErrorCode.DatabaseLoadFailed)
                );

                throw new DbLoadException("Can't load database");
            }
        }

        private DbConnection Connect(string dbFileName, string connectionString)
        {
            _databasePassword = GetDatabasePassword(dbFileName, connectionString);

            var connection = new SqliteConnection(connectionString);
            connection.Open();
            connection.Secure(_databasePassword);

            _logger.LogInformation($"Database connection done");

            return connection;
        }

        private void DeleteDatabaseFile(string dbFilePath)
        {
            try
            {
                File.Delete(dbFilePath);
            }
            catch (Exception exception) when (exception is DirectoryNotFoundException ||
                                              exception is UnauthorizedAccessException)
            {
                _logger.LogError(
                    exception,
                    _errorManager.GetFullErrorMessage(ErrorCode.DatabaseFileDeletionFailed)
                );
            }
        }

        public void Dispose()
        {
            DbContext?.Dispose();

            _dbConnection?.Close();
            _dbConnection?.Dispose();
        }
    }
}
