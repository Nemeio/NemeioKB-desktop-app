using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.DataModels;
using Nemeio.Core.PackageUpdater;
using Nemeio.Core.PackageUpdater.Informations;
using Nemeio.Core.Services;
using NUnit.Framework;

namespace Nemeio.Core.Test.PackageUpdater
{
    public class PackageUpdateCheckerShould
    {
        [Test]
        public void PackageUpdateChecker_Constructor_Ok()
        {
            var loggerFactory = new LoggerFactory();
            var mockIInformationService = Mock.Of<IInformationService>();
            var mockIPackageUpdaterRepository = Mock.Of<IPackageUpdateRepository>();

            Assert.DoesNotThrow(() => new PackageUpdateChecker(loggerFactory, mockIInformationService, mockIPackageUpdaterRepository));
            Assert.Throws<ArgumentNullException>(() => new PackageUpdateChecker(null, mockIInformationService, mockIPackageUpdaterRepository));
            Assert.Throws<ArgumentNullException>(() => new PackageUpdateChecker(loggerFactory, null, mockIPackageUpdaterRepository));
            Assert.Throws<ArgumentNullException>(() => new PackageUpdateChecker(loggerFactory, mockIInformationService, null));
            Assert.Throws<ArgumentNullException>(() => new PackageUpdateChecker(null, null, null));
        }

        [Test]
        public async Task PackageUpdateChecker_ApplicationNeedUpdateAsync_WhenNoUpdateAvailable_ThrowNoUpdateAvailableException()
        {
            const string noUpdateVersion = "1.0.0.0";

            var loggerFactory = new LoggerFactory();

            var mockIInformationService = Mock.Of<IInformationService>();
            Mock.Get(mockIInformationService)
                .Setup(x => x.GetApplicationVersion())
                .Returns(new VersionProxy(noUpdateVersion));

            var mockIPackageUpdaterRepository = Mock.Of<IPackageUpdateRepository>();
            Mock.Get(mockIPackageUpdaterRepository)
                .Setup((x) => x.GetApplicationPackageInformationAsync())
                .Returns(Task.FromResult(new DownloadablePackageInformation(new Version(noUpdateVersion), "this_is_not_a_checksum", new Uri("https://www.google.fr"))));

            var checker = new PackageUpdateChecker(loggerFactory, mockIInformationService, mockIPackageUpdaterRepository);
            var package = await checker.ApplicationNeedUpdateAsync();
           
            package.Should().BeNull();
        }

        [Test]
        public async Task PackageUpdateChecker_ApplicationNeedUpdateAsync_WhenUpdateIsAvailable_ReturnPackage()
        {
            var loggerFactory = new LoggerFactory();

            var mockIInformationService = Mock.Of<IInformationService>();
            Mock.Get(mockIInformationService)
                .Setup(x => x.GetApplicationVersion())
                .Returns(new VersionProxy("1.0.0.0"));

            var mockIPackageUpdaterRepository = Mock.Of<IPackageUpdateRepository>();
            Mock.Get(mockIPackageUpdaterRepository)
                .Setup((x) => x.GetApplicationPackageInformationAsync())
                .Returns(Task.FromResult(new DownloadablePackageInformation(new Version("2.0.0.0"), "this_is_not_a_checksum", new Uri("https://www.google.fr"))));

            var checker = new PackageUpdateChecker(loggerFactory, mockIInformationService, mockIPackageUpdaterRepository);
            var package = await checker.ApplicationNeedUpdateAsync();
           
            package.Should().NotBeNull();
        }
    }
}
