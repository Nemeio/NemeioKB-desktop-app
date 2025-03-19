using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Errors;
using Nemeio.Core.FileSystem;
using Nemeio.Core.PackageUpdater;
using Nemeio.Core.Services;
using NUnit.Framework;

namespace Nemeio.Core.Test.PackageUpdater
{
    public class PackageUpdaterFileProviderShould
    {
        [Test]
        public void PackageUpdater_Constructor_Ok()
        {
            var mockLoggerFactory = Mock.Of<ILoggerFactory>();
            var mockDocumentService = Mock.Of<IDocument>();
            var mockErrorManager = Mock.Of<IErrorManager>();
            var mockFileSystem = Mock.Of<IFileSystem>();

            Assert.DoesNotThrow(() => new PackageUpdaterFileProvider(mockLoggerFactory, mockDocumentService, mockErrorManager, mockFileSystem));
            Assert.Throws<ArgumentNullException>(() => new PackageUpdaterFileProvider(null, mockDocumentService, mockErrorManager, mockFileSystem));
            Assert.Throws<ArgumentNullException>(() => new PackageUpdaterFileProvider(mockLoggerFactory, null, mockErrorManager, mockFileSystem));
            Assert.Throws<ArgumentNullException>(() => new PackageUpdaterFileProvider(mockLoggerFactory, mockDocumentService, null, mockFileSystem));
            Assert.Throws<ArgumentNullException>(() => new PackageUpdaterFileProvider(mockLoggerFactory, mockDocumentService, mockErrorManager, null));
            Assert.Throws<ArgumentNullException>(() => new PackageUpdaterFileProvider(null, null, null, null));
        }

        [Test]
        public void PackageUpdater_GetUpdateFilePath_WhenPackageIsNull_ThrowsArgumentNullException()
        {
            var mockLoggerFactory = Mock.Of<ILoggerFactory>();
            var mockDocumentService = Mock.Of<IDocument>();
            var mockErrorManager = Mock.Of<IErrorManager>();
            var mockFileSystem = Mock.Of<IFileSystem>();

            var fileProvider = new PackageUpdaterFileProvider(mockLoggerFactory, mockDocumentService, mockErrorManager, mockFileSystem);

            Assert.Throws<ArgumentNullException>(() =>
            {
                fileProvider.GetUpdateFilePath(null);
            });
        }
    }
}
