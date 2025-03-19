using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Nemeio.Core.DataModels;
using Nemeio.Core.Errors;
using Nemeio.Core;
using NUnit.Framework;

namespace Nemeio.Core.Test.Managers
{
    [TestFixture]
    public class DownloaderShould
    {
        public static string DownloaderUnitTestUrl = NemeioConstants.ServerUrl + "/UnitTest/downloader_unit_test.txt";
        public static string DownloaderUnitTestChecksum = "13614a11c9bf9efb33559910c64d7fa4";
        public static VersionProxy Version1 = new VersionProxy(versionString);
        public static Updater DownloaderUnitTestUpdater;

        private readonly ILoggerFactory _loggerFactory = new LoggerFactory();
        private readonly IErrorManager _errorManager = new ErrorManager();
        private const string updatePackage = "MyDownload.zip";
        private const string fakeUrl = "http://Google.fr/";
        private const string badUrl = @"http:\\\host/path/file";
        private const string fakeChecksum = "123456789abcdef";
        private const string versionString = "1.0.0";

        [SetUp]
        public void SetUp()
        {
            DownloaderUnitTestUpdater = new Updater(DownloaderUnitTestUrl, Version1, UpdateType.App, DownloaderUnitTestChecksum);
        }

        /// Constructor

        [Test]
        public void Downloader_01_01_Constructor_IncompleteInstallerPath_ThrowsArgumentException()
        {
            var updater = new Updater(fakeUrl + updatePackage, Version1, UpdateType.App, fakeChecksum);
            Assert.Throws<ArgumentException>(() => new Core.Managers.Downloader(_loggerFactory, new HttpClient(), updater, _errorManager));
        }

        [Test]
        public void Downloader_01_02_Constructor_CompleteInstallerPath_BuildsOk()
        {
            var updater = new Updater(fakeUrl + updatePackage, Version1, UpdateType.App, fakeChecksum);
            updater.ComputeInstallerPath(Path.GetTempPath());
            var downloader = new Core.Managers.Downloader(_loggerFactory, new HttpClient(), updater, _errorManager);
        }

        /// Download

        [Test]
        public async Task Downloader_02_01_Download_NotExistingInstallerPath_ReturnsFalseErrorMessage()
        {
            var updater = new Updater(fakeUrl + updatePackage, Version1, UpdateType.App, fakeChecksum);
            updater.ComputeInstallerPath(Path.GetTempPath());
            var downloader = new Core.Managers.Downloader(_loggerFactory, new HttpClient(), updater, _errorManager);
            Assert.IsNull(downloader.LastError);

            var result = await downloader.Download();
            result.Should().BeFalse();
            downloader.LastError.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task Downloader_02_02_Download_ValidInstallerPathInvalidCheckSum_ReturnsFalseErrorMessage()
        {
            var updater = new Updater(NemeioConstants.UpdatesUrl, Version1, UpdateType.App, fakeChecksum);
            updater.ComputeInstallerPath(Path.GetTempPath());
            var downloader = new Core.Managers.Downloader(_loggerFactory, new HttpClient(), updater, _errorManager);

            var result = await downloader.Download();
            Assert.IsFalse(result);
            Assert.IsNotNull(downloader.LastError);
            downloader.LastError.Should().NotBeNullOrEmpty();
            Assert.True(File.Exists(updater.InstallerPath));
            File.Delete(updater.InstallerPath);
        }

        [Test]
        public async Task Downloader_02_03_Download_ValidInstallerPath_ReturnsTrue()
        {
            var updater = DownloaderUnitTestUpdater;
            updater.ComputeInstallerPath(Path.GetTempPath());

            var updaterInstallerPath = updater.InstallerPath;
            if (File.Exists(updaterInstallerPath))
            {
                File.Delete(updaterInstallerPath);
            }

            var downloader = new Core.Managers.Downloader(_loggerFactory, new FakeHttpClient(), updater, _errorManager);

            var result = await downloader.Download();
            Assert.IsTrue(result);
            Assert.IsNull(downloader.LastError);
            Assert.True(File.Exists(updaterInstallerPath));
            File.Delete(updaterInstallerPath);
        }
    }

    internal class FakeHttpClient : IHttpClient
    {
        public async Task DownloadFileTaskAsync(Uri url, string filename)
        {
            await Task.Yield();

            //  In out case we just copy file from place to another
            var currentDirectory = Directory.GetCurrentDirectory();
            var resourcePath = Path.Combine(currentDirectory, "Resources", "downloader_unit_test.txt");

            File.Copy(resourcePath, filename);
        }

        public void Dispose() { }
    }
}
