using System;
using System.Threading.Tasks;

namespace Nemeio.Keyboard.Communication.Adapters
{
    public interface IBluetoothLEAdapter
    {
        event EventHandler DataReceived;
        int BytesToRead { get; }
        Task ConnectAsync(string identifier);
        Task DisconnectAsync();
        int Read(byte[] buffer, int offset, int count);
        void Write(byte[] buffer, int offset, int count);
    }
}
