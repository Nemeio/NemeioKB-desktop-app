using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Communication.Exceptions;
using Nemeio.Core.Services.Layouts;
using Nemeio.Keyboard.Communication.Protocol.v1.Monitors;
using Nemeio.Keyboard.Communication.Tools.Frames;
using NUnit.Framework;

namespace Nemeio.Keyboard.Communication.Tests.Monitors
{
    [TestFixture]
    public class AddConfigurationMonitorShould
    {
        [Test]
        public void AddConfigurationMonitor_SendConfiguration_WithNullParameter_Throws()
        {
            var loggerFactory = new LoggerFactory();
            var keyboardCommandFactory = Mock.Of<IKeyboardCommandFactory>();
            var keyboardCommandExecutor = Mock.Of<IKeyboardCommandExecutor>();
            var keyboardErrorConverter = Mock.Of<IKeyboardErrorConverter>();

            var addConfigurationMonitor = new AddConfigurationMonitor(loggerFactory, keyboardCommandFactory, keyboardCommandExecutor, keyboardErrorConverter);

            Assert.Throws<ArgumentNullException>(() => addConfigurationMonitor.SendConfiguration(null));
        }

        [Test]
        public async Task AddConfigurationMonitor_SendConfiguration_WithCommunicationTimeout_Throws()
        {
            var loggerFactory = new LoggerFactory();
            var layout = Mock.Of<ILayout>();
            var keyboardCommandFactory = Mock.Of<IKeyboardCommandFactory>();
            var keyboardCommandExecutor = Mock.Of<IKeyboardCommandExecutor>();
            var keyboardErrorConverter = Mock.Of<IKeyboardErrorConverter>();

            var continueSemaphore = new SemaphoreSlim(0, 1);

            var sendConfigurationCommand = Mock.Of<IKeyboardCommand>();
            Mock.Get(sendConfigurationCommand)
                .Setup(x => x.FrameCount)
                .Returns(1);

            Mock.Get(keyboardCommandFactory)
                .Setup(x => x.CreateSendConfigurationCommand(It.IsAny<ILayout>(), false))
                .Returns(sendConfigurationCommand);

            Mock.Get(keyboardCommandExecutor)
                .Setup(x => x.ScheduleCommand(It.IsAny<IKeyboardCommand>()))
                .Callback(() => continueSemaphore.Release())
                .Returns(true);

            var addConfigurationMonitor = new AddConfigurationMonitor(loggerFactory, keyboardCommandFactory, keyboardCommandExecutor, keyboardErrorConverter);

            //  No wait this task
            //  Use semaphore
            var sendConfigurationTask = Task.Run(() =>
            {
                addConfigurationMonitor.SendConfiguration(layout);
            });

            await continueSemaphore.WaitAsync();

            addConfigurationMonitor.ReceiveResponse(null, new CommunicationTimeoutException());

            Assert.ThrowsAsync<CommandFailedException>(() => sendConfigurationTask);
        }

        [Test]
        public async Task AddConfigurationMonitor_SendConfiguration_WithValidResponseFromKeyboard_Ok()
        {
            var loggerFactory = new LoggerFactory();
            var layout = Mock.Of<ILayout>();
            var keyboardCommandFactory = Mock.Of<IKeyboardCommandFactory>();
            var keyboardCommandExecutor = Mock.Of<IKeyboardCommandExecutor>();
            var keyboardErrorConverter = Mock.Of<IKeyboardErrorConverter>();

            var continueSemaphore = new SemaphoreSlim(0, 1);

            var sendConfigurationCommand = Mock.Of<IKeyboardCommand>();
            Mock.Get(sendConfigurationCommand)
                .Setup(x => x.FrameCount)
                .Returns(1);

            Mock.Get(keyboardCommandFactory)
                .Setup(x => x.CreateSendConfigurationCommand(It.IsAny<ILayout>(), false))
                .Returns(sendConfigurationCommand);

            Mock.Get(keyboardCommandExecutor)
                .Setup(x => x.ScheduleCommand(It.IsAny<IKeyboardCommand>()))
                .Callback(() => continueSemaphore.Release())
                .Returns(true);

            var validFrame = new SerialFrame(Core.Device.CommandId.SendData, new byte[] { (byte)KeyboardErrorCode.Success });
            var addConfigurationMonitor = new AddConfigurationMonitor(loggerFactory, keyboardCommandFactory, keyboardCommandExecutor, keyboardErrorConverter);

            //  No wait this task
            //  Use semaphore
            var sendConfigurationTask = Task.Run(() =>
            {
                addConfigurationMonitor.SendConfiguration(layout);
            });

            await continueSemaphore.WaitAsync();

            addConfigurationMonitor.ReceiveResponse(validFrame, null);

            Assert.DoesNotThrowAsync(() => sendConfigurationTask);
        }
    }
}
