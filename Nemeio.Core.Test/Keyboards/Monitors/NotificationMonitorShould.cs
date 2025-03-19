using System;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Monitors;
using NUnit.Framework;

namespace Nemeio.Core.Test.Keyboards.Monitors
{
    [TestFixture]
    public class NotificationMonitorShould
    {
        private class TestableNotificationMonitor : NotificationMonitor
        {
            public TestableNotificationMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter) 
                : base(loggerFactory, commandFactory, commandExecutor, errorConverter) { }

            public void PublicExecuteCommand(IKeyboardCommand command) => ExecuteCommand(command);

            public override void OnReceiveNotification(KeyboardResponse response)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void NotificationMonitor_ExecuteCommand_ThrowsNotSupportedException()
        {
            var loggerFactory = new LoggerFactory();
            var commandFactory = Mock.Of<IKeyboardCommandFactory>();
            var commandExecutor = Mock.Of<IKeyboardCommandExecutor>();
            var errorConverter = Mock.Of<IKeyboardErrorConverter>();

            var monitor = new TestableNotificationMonitor(loggerFactory, commandFactory, commandExecutor, errorConverter);

            var mockKeyboardCommand = Mock.Of<IKeyboardCommand>();

            Assert.Throws<NotSupportedException>(() => monitor.PublicExecuteCommand(mockKeyboardCommand));
        }
    }
}
