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
    public class ApplicationKeyExecutorShould
    {
        [Test]
        public void ApplicationKeyExecutor_Constructor_Ok()
        {
            var loggerFactory = new LoggerFactory();
            var mockErrorManager = Mock.Of<IErrorManager>();
            var mockBrowserFile = Mock.Of<IBrowserFile>();
            var mockDialogService = Mock.Of<IDialogService>();

            Assert.Throws<ArgumentNullException>(() => new ApplicationKeyExecutor(null, mockBrowserFile, mockDialogService, mockErrorManager, string.Empty));
            Assert.Throws<ArgumentNullException>(() => new ApplicationKeyExecutor(loggerFactory, null, mockDialogService, mockErrorManager, string.Empty));
            Assert.Throws<ArgumentNullException>(() => new ApplicationKeyExecutor(loggerFactory, mockBrowserFile, null, mockErrorManager, string.Empty));
            Assert.Throws<ArgumentNullException>(() => new ApplicationKeyExecutor(loggerFactory, mockBrowserFile, mockDialogService, null, string.Empty));
            Assert.Throws<ArgumentNullException>(() => new ApplicationKeyExecutor(loggerFactory, mockBrowserFile, mockDialogService, null, null));

            Assert.DoesNotThrow(() => new ApplicationKeyExecutor(loggerFactory, mockBrowserFile, mockDialogService, mockErrorManager, string.Empty));
        }

        [Test]
        public async Task ApplicationKeyExecutor_Execute_WhenDataIsValid()
        {
            const string applicationPath = @"C:\Windows\System32\notepad.exe";

            var openApplicationCalled = false;

            var loggerFactory = new LoggerFactory();
            var mockErrorManager = Mock.Of<IErrorManager>();
            var mockBrowserFile = Mock.Of<IBrowserFile>();
            Mock.Get(mockBrowserFile)
                .Setup(x => x.OpenApplication(It.IsAny<string>()))
                .Callback(() => openApplicationCalled = true);

            var mockDialogService = Mock.Of<IDialogService>();

            var executor = new ApplicationKeyExecutor(loggerFactory, mockBrowserFile, mockDialogService, mockErrorManager, applicationPath);

            await executor.ExecuteAsync();

            openApplicationCalled.Should().BeTrue();
        }

        [Test]
        public async Task ApplicationKeyExecutor_Execute_WhenDataIsInvalid()
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

            var executor = new ApplicationKeyExecutor(loggerFactory, mockBrowserFile, mockDialogService, mockErrorManager, string.Empty);

            await executor.ExecuteAsync();

            openApplicationCalled.Should().BeFalse();
            dialogShown.Should().BeTrue();
            errorCode.Should().NotBeNull();
            errorCode.Should().Be(ErrorCode.CoreOpenApplicationFailed);
        }
    }
}
