using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Keyboard.Builds;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Frame;
using Nemeio.Core.Keyboard.Configurations.Changed;
using Nemeio.Core.Keyboard.KeyboardFailures;
using Nemeio.Core.Keyboard.Keys;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Keyboard.Updates.Progress;
using Nemeio.Core.Tools.Retry;
using NUnit.Framework;

namespace Nemeio.Core.Test.Keyboards.Builds
{
    [TestFixture]
    public class NemeioFactoryShould
    {
        [Test]
        public void NemeioFactory_CreateRunner_WithNullParameters_Throws()
        {
            var loggerFactory = new LoggerFactory();
            var monitorFactory = Mock.Of<IMonitorFactory>();
            var frameParser = Mock.Of<IFrameParser>();
            var crashLogger = Mock.Of<IKeyboardCrashLogger>();
            var retryHandler = Mock.Of<IRetryHandler>();
            var screenFactory = Mock.Of<IScreenFactory>();

            var factory = new NemeioFactory(loggerFactory, monitorFactory, frameParser, crashLogger, retryHandler, screenFactory);

            Assert.Throws<ArgumentNullException>(() => factory.CreateRunner(null));
        }

        [Test]
        public void NemeioFactory_CreateRunner_Ok()
        {
            var keyboardId = "myId";
            var keyboardVersion = new Version("2.0");
            var keyboardCommunicationType = CommunicationType.Serial;
            var io = Mock.Of<IKeyboardIO>();
            var screenFactory = Mock.Of<IScreenFactory>();

            var loggerFactory = new LoggerFactory();
            var monitorFactory = Mock.Of<IMonitorFactory>();
            Mock.Get(monitorFactory)
                .Setup(x => x.CreateConfigurationChangedMonitor(It.IsAny<IKeyboardCommandExecutor>()))
                .Returns(Mock.Of<IConfigurationChangedMonitor>());

            Mock.Get(monitorFactory)
                .Setup(x => x.CreateKeyPressedMonitor(It.IsAny<IKeyboardCommandExecutor>()))
                .Returns(Mock.Of<IKeyPressedMonitor>());

            var frameParser = Mock.Of<IFrameParser>();
            var crashLogger = Mock.Of<IKeyboardCrashLogger>();
            var retryHandler = Mock.Of<IRetryHandler>();

            var factory = new NemeioFactory(loggerFactory, monitorFactory, frameParser, crashLogger, retryHandler, screenFactory);
            var keyboard = new Keyboard.Keyboard(keyboardId, keyboardVersion, keyboardCommunicationType, io);

            var nemeio = factory.CreateRunner(keyboard);
            
            nemeio.Should().NotBeNull();
            nemeio.Identifier.Should().Be(keyboardId);
            nemeio.CommunicationType.Should().Be(keyboardCommunicationType);
        }

        [Test]
        public void NemeioFactory_CreateUpdater_WithNullParameters_Throws()
        {
            var loggerFactory = new LoggerFactory();
            var monitorFactory = Mock.Of<IMonitorFactory>();
            var frameParser = Mock.Of<IFrameParser>();
            var crashLogger = Mock.Of<IKeyboardCrashLogger>();
            var retryHandler = Mock.Of<IRetryHandler>();
            var screenFactory = Mock.Of<IScreenFactory>();

            var factory = new NemeioFactory(loggerFactory, monitorFactory, frameParser, crashLogger, retryHandler, screenFactory);

            Assert.Throws<ArgumentNullException>(() => factory.CreateUpdater(null));
        }

        [Test]
        public void NemeioFactory_CreateUpdater_Ok()
        {
            var keyboardId = "myId";
            var keyboardVersion = new Version("2.0");
            var keyboardCommunicationType = CommunicationType.Serial;
            var io = Mock.Of<IKeyboardIO>();
            var screenFactory = Mock.Of<IScreenFactory>();

            var loggerFactory = new LoggerFactory();
            var updateProgressMonitor = Mock.Of<IUpdateProgressMonitor>();
            var monitorFactory = Mock.Of<IMonitorFactory>();
            Mock.Get(monitorFactory)
                .Setup(x => x.CreateUpdateProgressMonitor(It.IsAny<IKeyboardCommandExecutor>()))
                .Returns(updateProgressMonitor);

            var frameParser = Mock.Of<IFrameParser>();
            var crashLogger = Mock.Of<IKeyboardCrashLogger>();
            var retryHandler = Mock.Of<IRetryHandler>();

            var factory = new NemeioFactory(loggerFactory, monitorFactory, frameParser, crashLogger, retryHandler, screenFactory);
            var keyboard = new Keyboard.Keyboard(keyboardId, keyboardVersion, keyboardCommunicationType, io);

            var nemeio = factory.CreateUpdater(keyboard);

            nemeio.Should().NotBeNull();
            nemeio.Identifier.Should().Be(keyboardId);
            nemeio.CommunicationType.Should().Be(keyboardCommunicationType);
        }

        [Test]
        public void NemeioFactory_CreateVersionChecker_WithNullParameters_Throws()
        {
            var loggerFactory = new LoggerFactory();
            var monitorFactory = Mock.Of<IMonitorFactory>();
            var frameParser = Mock.Of<IFrameParser>();
            var crashLogger = Mock.Of<IKeyboardCrashLogger>();
            var retryHandler = Mock.Of<IRetryHandler>();
            var screenFactory = Mock.Of<IScreenFactory>();

            var factory = new NemeioFactory(loggerFactory, monitorFactory, frameParser, crashLogger, retryHandler, screenFactory);

            Assert.Throws<ArgumentNullException>(() => factory.CreateVersionChecker(null));
        }

        [Test]
        public void NemeioFactory_CreateVersionChecker_Ok()
        {
            var keyboardId = "myId";
            var keyboardVersion = new Version("2.0");
            var keyboardCommunicationType = CommunicationType.Serial;
            var io = Mock.Of<IKeyboardIO>();

            var loggerFactory = new LoggerFactory();
            var monitorFactory = Mock.Of<IMonitorFactory>();
            var frameParser = Mock.Of<IFrameParser>();
            var crashLogger = Mock.Of<IKeyboardCrashLogger>();
            var retryHandler = Mock.Of<IRetryHandler>();
            var screenFactory = Mock.Of<IScreenFactory>();

            var factory = new NemeioFactory(loggerFactory, monitorFactory, frameParser, crashLogger, retryHandler, screenFactory);
            var keyboard = new Keyboard.Keyboard(keyboardId, keyboardVersion, keyboardCommunicationType, io);

            var nemeio = factory.CreateVersionChecker(keyboard);

            nemeio.Should().NotBeNull();
            nemeio.Identifier.Should().Be(keyboardId);
            nemeio.CommunicationType.Should().Be(keyboardCommunicationType);
        }
    }
}
