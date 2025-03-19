using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Acl.HttpComm.Connectivity;
using Nemeio.Core.Services;
using NUnit.Framework;

namespace Nemeio.Acl.Test.Connectivity
{
    public class NetworkConnectivityCheckerShould
    {
        [Test]
        public void NetworkConnectivityChecker_Constructor_Ok()
        {
            var loggerFactory = new LoggerFactory();
            var mockHttpService = Mock.Of<IHttpService>();

            Assert.Throws<ArgumentNullException>(() => new NetworkConnectivityChecker(loggerFactory, null));
            Assert.Throws<ArgumentNullException>(() => new NetworkConnectivityChecker(null, mockHttpService));
            Assert.Throws<ArgumentNullException>(() => new NetworkConnectivityChecker(null, null));
            Assert.DoesNotThrow(() => new NetworkConnectivityChecker(loggerFactory, mockHttpService));
        }

        [Test]
        public void NetworkConnectivityChecker_Constructor_CheckIntervalValue_Ok()
        {
            var loggerFactory = new LoggerFactory();
            var mockHttpService = Mock.Of<IHttpService>();

            var checker = new NetworkConnectivityChecker(loggerFactory, mockHttpService);

            checker.CheckInterval.Should().Be(NetworkConnectivityChecker.Timeout);
        }

        [Test]
        public void NetworkConnectivityChecker_AutomaticCheck_Ok()
        {
            var loggerFactory = new LoggerFactory();
            var mockHttpService = Mock.Of<IHttpService>();

            Mock.Get(mockHttpService)
                .Setup(x => x.SendAsync<string>(It.IsAny<string>(), HttpMethod.Get, null, null, true))
                .Returns(Task.FromResult(string.Empty));

            var checker = new NetworkConnectivityChecker(loggerFactory, mockHttpService);
            checker.CheckInterval = 5;
            checker.ConnectivityStatusChanged += delegate
            {
                checker.InternetAvailable.Should().BeTrue();
            };
        }
    }
}
