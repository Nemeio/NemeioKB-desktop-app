namespace Nemeio.Core.Keyboard.Updates
{
    public interface IUpdateMonitor
    {
        void SendFirmware(byte[] firmware);
    }
}
