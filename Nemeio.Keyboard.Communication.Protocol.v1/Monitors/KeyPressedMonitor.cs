using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Device;
using Nemeio.Core.JsonModels;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Keys;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Keyboard.Communication.Protocol.v1.Utils;
using Nemeio.Keyboard.Communication.Tools.Utils;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Monitors
{
    public class KeyPressedMonitor : NotificationMonitor, IKeyPressedMonitor
    {
        public event EventHandler<KeyPressedEventArgs> OnKeyPressed;

        public KeyPressedMonitor(ILoggerFactory loggerFactory, IKeyboardCommandFactory commandFactory, IKeyboardCommandExecutor commandExecutor, IKeyboardErrorConverter errorConverter) 
            : base(loggerFactory, commandFactory, commandExecutor, errorConverter)
        {
            commandExecutor.RegisterNotification(CommandId.KeyPressed, this);
        }

        public override void OnReceiveNotification(KeyboardResponse response)
        {
            var keystrokes = BytesToNemeioKeystrokes(response.Frame.Payload);

            OnKeyPressed?.Invoke(this, new KeyPressedEventArgs(keystrokes));
        }

        private NemeioIndexKeystroke[] BytesToNemeioKeystrokes(byte[] packetBytes)
        {
            NemeioIndexKeystroke[] keystrokes;

            using (var mPayload = new MemoryStream(packetBytes))
            using (var bPayload = new BinaryReader(mPayload))
            {
                sbyte count = bPayload.ReadSByte();

                if (count <= 0)
                {
                    return new NemeioIndexKeystroke[0];
                }

                keystrokes = new NemeioIndexKeystroke[count];

                for (int i = 0; i < count; i++)
                {
                    const int size = 4;
                    var action = KeyboardProtocolHelpers.ToBigEndian(bPayload.ReadBytes(size), size);

                    keystrokes[i] = new NemeioIndexKeystroke()
                    {
                        Index = BitConverter.ToInt32(action, 0)
                    };
                }
            }

            return keystrokes;
        }
    }
}
