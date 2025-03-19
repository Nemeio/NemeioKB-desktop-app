using System;
using System.Linq;
using FluentAssertions;
using Nemeio.Core.Device;
using Nemeio.Keyboard.Communication.Protocol.v1.Commands;
using NUnit.Framework;

namespace Nemeio.Keyboard.Communication.Tests.Commands
{
    [TestFixture]
    public class EmptyPayloadCommandShould
    {
        private class TestableEmptyPayloadCommand : EmptyPayloadCommand
        {
            public TestableEmptyPayloadCommand(CommandId commandId) 
                : base(commandId) { }
        }

        public static Array CommandIds() => Enum.GetValues(typeof(CommandId));

        [TestCaseSource("CommandIds")]
        public void EmptyPayloadCommand_ToFrames_Ok(CommandId commandId)
        {
            var emptyPayloadCommand = new TestableEmptyPayloadCommand(commandId);

            emptyPayloadCommand.CommandId.Should().Be(commandId);
            emptyPayloadCommand.FrameCount.Should().Be(1);

            var frames = emptyPayloadCommand.ToFrames();

            frames.Should().NotBeNull();
            frames.Should().NotBeEmpty();
            frames.Count.Should().Be(1);
            frames.First().CommandId.Should().Be(commandId);
            frames.First().Payload.Should().BeEmpty();
        }
    }
}
