using System;
using System.Threading.Tasks;

namespace Nemeio.Core.Keyboard.Communication
{
    public interface IKeyboardIO
    {
        int BytesToRead { get; }

        event EventHandler OnDataReceived;

        void Write(byte[] buffer, int offset, int count);

        int Read(byte[] buffer, int offset, int count);

        Task ConnectAsync(string idenfifier);

        Task DisconnectAsync();
    }
}
