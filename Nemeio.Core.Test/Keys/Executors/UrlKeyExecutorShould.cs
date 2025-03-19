using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Errors;
using Nemeio.Core.Keys.Executors;
using Nemeio.Core.Services;
using NUnit.Framework;

namespace Nemeio.Core.Test.Keys.Executors
{
    public class UrlKeyExecutorShould
    {
        [Test]
        public void UrlKeyExecutor_Constructor_Ok()
        {
            var loggerFactory = new LoggerFactory();
            var mockErrorManager = Mock.Of<IErrorManager>();
            var mockBrowserFile = Mock.Of<IBrowserFile>();
            var mockDialogService = Mock.Of<IDialogService>();

            Assert.Throws<ArgumentNullException>(() => new UrlKeyExecutor(null, mockBrowserFile, mockDialogService, mockErrorManager, string.Empty));
            Assert.Throws<ArgumentNullException>(() => new UrlKeyExecutor(loggerFactory, null, mockDialogService, mockErrorManager, string.Empty));
            Assert.Throws<ArgumentNullException>(() => new UrlKeyExecutor(loggerFactory, mockBrowserFile, null, mockErrorManager, string.Empty));
            Assert.Throws<ArgumentNullException>(() => new UrlKeyExecutor(loggerFactory, mockBrowserFile, mockDialogService, null, string.Empty));
            Assert.Throws<ArgumentNullException>(() => new UrlKeyExecutor(loggerFactory, mockBrowserFile, mockDialogService, null, null));

            Assert.DoesNotThrow(() => new UrlKeyExecutor(loggerFactory, mockBrowserFile, mockDialogService, mockErrorManager, string.Empty));
        }

        [Test]
        public async Task UrlKeyExecutor_Execute_WhenDataIsValid()
        {
            const string url = @"https://www.google.fr";

            var openUrlCalled = false;

            var loggerFactory = new LoggerFactory();
            var mockErrorManager = Mock.Of<IErrorManager>();
            var mockBrowserFile = Mock.Of<IBrowserFile>();
            Mock.Get(mockBrowserFile)
                .Setup(x => x.OpenUrl(It.IsAny<string>()))
                .Callback(() => openUrlCalled = true);

            var mockDialogService = Mock.Of<IDialogService>();

            var executor = new UrlKeyExecutor(loggerFactory, mockBrowserFile, mockDialogService, mockErrorManager, url);

            await executor.ExecuteAsync();

            openUrlCalled.Should().BeTrue();
        }

        [Test]
        public async Task UrlKeyExecutor_Execute_WhenDataIsInvalid()
        {
            var openApplicationCalled = false;
            var dialogShown = false;
            ErrorCode? errorCode = null;

            var loggerFactory = new LoggerFactory();

            var mockBrowserFile = Mock.Of<IBrowserFile>();
            Mock.Get(mockBrowserFile)
                .Setup(x => x.OpenApplication(It.IsAny<string>()))
                .Callback(() => openApplicationCalled = true);

            var mockDialogService = Mock.Of<IDialogService>();
            Mock.Get(mockDialogService)
                .Setup(x => x.ShowMessageAsync(It.IsAny<StringId>(), It.IsAny<StringId>()))
                .Callback(() => dialogShown = true);

            var mockErrorManager = Mock.Of<IErrorManager>();
            Mock.Get(mockErrorManager)
                .Setup(x => x.GetFullErrorMessage(It.IsAny<ErrorCode>(), It.IsAny<Exception>()))
                .Callback<ErrorCode, Exception>((errCode, ex) => errorCode = errCode);

            var executor = new UrlKeyExecutor(loggerFactory, mockBrowserFile, mockDialogService, mockErrorManager, string.Empty);

            await executor.ExecuteAsync();

            openApplicationCalled.Should().BeFalse();
            dialogShown.Should().BeTrue();
            errorCode.Should().NotBeNull();
            errorCode.Should().Be(ErrorCode.CoreOpenUrlFailed);
        }
    }
}
