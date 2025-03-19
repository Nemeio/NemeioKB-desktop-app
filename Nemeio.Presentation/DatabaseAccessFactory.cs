using Microsoft.Extensions.Logging;
using Nemeio.Core.Errors;
using Nemeio.Core.Services;
using Nemeio.Infrastructure;

namespace Nemeio.Presentation
{
    public class DatabaseAccessFactory : IDatabaseAccessFactory
    {
        private ILoggerFactory _loggerFactory;
        private IProtectedDataProvider _protectedDataProvider;
        private IDocument _documentService;
        private IErrorManager _errorManager;

        public DatabaseAccessFactory(ILoggerFactory loggerFactory, IProtectedDataProvider protectedDataProvider, IDocument documentService, IErrorManager errorManager)
        {
            _loggerFactory = loggerFactory;
            _protectedDataProvider = protectedDataProvider;
            _documentService = documentService;
            _errorManager = errorManager;
        }

        public DbAccess CreateDatabaseAccess()
        {
            return new DbAccess(_loggerFactory, _protectedDataProvider, _documentService, _errorManager);
        }
    }
}
