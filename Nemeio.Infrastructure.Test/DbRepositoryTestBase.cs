using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Errors;
using Nemeio.Core.Services;
using Nemeio.Presentation;
using NUnit.Framework;

namespace Nemeio.Infrastructure.Test
{
    [TestFixture]
    public abstract class DbRepositoryTestBase
    {
        public static readonly string FileName = "nemeio.test.db";

        private IDocument _document;
        private IProtectedDataProvider _protectedDataProvider;
        protected IDatabaseAccessFactory _databaseAccessFactory;
        private string testDatabasePath = Path.Combine(Directory.GetCurrentDirectory(), FileName);

        [SetUp]
        public virtual void SetUp()
        {
            RemoveDatabaseIfExists();

            _document = Mock.Of<IDocument>();
            Mock.Get(_document)
                .Setup(mock => mock.DatabasePath)
                .Returns(testDatabasePath);

            _protectedDataProvider = Mock.Of<IProtectedDataProvider>();
            Mock.Get(_protectedDataProvider)
                .Setup(mock => mock.GetPassword(It.IsAny<string>()))
                .Returns("password");

            _databaseAccessFactory = new DatabaseAccessFactory(new LoggerFactory(), _protectedDataProvider, _document, new ErrorManager());

            using (var dbAccess = _databaseAccessFactory.CreateDatabaseAccess())
            {
                dbAccess.DbContext.Database.Migrate();
            }
        }

        [TearDown]
        public void TearDown() => RemoveDatabaseIfExists();

        private void RemoveDatabaseIfExists()
        {
            if (File.Exists(testDatabasePath))
            {
                File.Delete(testDatabasePath);
            }
        }
    }
}
