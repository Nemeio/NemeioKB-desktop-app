using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Keyboard.Communication.Monitors;
using NUnit.Framework;

namespace Nemeio.Keyboard.Communication.Tests.Monitors
{
    [TestFixture]
    internal class MonitorFactoryShould
    {
        [TestCase(0)]
        [TestCase(4)]
        public void MonitorFactory_CreateParametersMonitor_WhenProtocolVersionIsUnknown_Throws(int protocolVersion)
        {
            var loggerFactory = new LoggerFactory();
            var mockKeyboardCommandFactory = Mock.Of<IKeyboardCommandFactory>();
            var mockKeyboardErrorConverter = Mock.Of<IKeyboardErrorConverter>();
            var mockKeyboardCommandExecutor = Mock.Of<IKeyboardCommandExecutor>();
            var monitorFactory = new MonitorFactory(loggerFactory, mockKeyboardCommandFactory, mockKeyboardErrorConverter);

            var version = new System.Version($"2.{protocolVersion}");

            Assert.Throws<ArgumentOutOfRangeException>(() => monitorFactory.CreateParametersMonitor(version, mockKeyboardCommandExecutor));
        }

        [Test]
        public void MonitorFactory_CreateParametersMonitor_WhenProtocolVersionIsEqualsTo1_CreateProtocolv1Instance_Ok()
        {
            var loggerFactory = new LoggerFactory();
            var mockKeyboardCommandFactory = Mock.Of<IKeyboardCommandFactory>();
            var mockKeyboardErrorConverter = Mock.Of<IKeyboardErrorConverter>();
            var mockKeyboardCommandExecutor = Mock.Of<IKeyboardCommandExecutor>();
            var monitorFactory = new MonitorFactory(loggerFactory, mockKeyboardCommandFactory, mockKeyboardErrorConverter);

            var version = new System.Version($"2.1");
            var monitor = monitorFactory.CreateParametersMonitor(version, mockKeyboardCommandExecutor);

            monitor.Should().NotBeNull();
            monitor.Parser.GetType().Should().Be(typeof(Protocol.v1.Utils.KeyboardParameterConverter));
        }

        [Test]
        public void MonitorFactory_CreateParametersMonitor_WhenProtocolVersionIsEqualsTo2_CreateProtocolv2Instance_Ok()
        {
            var loggerFactory = new LoggerFactory();
            var mockKeyboardCommandFactory = Mock.Of<IKeyboardCommandFactory>();
            var mockKeyboardErrorConverter = Mock.Of<IKeyboardErrorConverter>();
            var mockKeyboardCommandExecutor = Mock.Of<IKeyboardCommandExecutor>();
            var monitorFactory = new MonitorFactory(loggerFactory, mockKeyboardCommandFactory, mockKeyboardErrorConverter);

            var version = new System.Version($"2.2");
            var monitor = monitorFactory.CreateParametersMonitor(version, mockKeyboardCommandExecutor);

            monitor.Should().NotBeNull();
            monitor.Parser.GetType().Should().Be(typeof(Protocol.v2.Utils.KeyboardParameterConverter));
        }

        [Test]
        public void MonitorFactory_CreateParametersMonitor_WhenProtocolVersionIsEqualsTo3_CreateProtocolv3Instance_Ok()
        {
            var loggerFactory = new LoggerFactory();
            var mockKeyboardCommandFactory = Mock.Of<IKeyboardCommandFactory>();
            var mockKeyboardErrorConverter = Mock.Of<IKeyboardErrorConverter>();
            var mockKeyboardCommandExecutor = Mock.Of<IKeyboardCommandExecutor>();
            var monitorFactory = new MonitorFactory(loggerFactory, mockKeyboardCommandFactory, mockKeyboardErrorConverter);

            var version = new System.Version($"2.3");
            var monitor = monitorFactory.CreateParametersMonitor(version, mockKeyboardCommandExecutor);

            monitor.Should().NotBeNull();
            monitor.Parser.GetType().Should().Be(typeof(Protocol.v3.Utils.KeyboardParameterConverter));
        }
    }
}
