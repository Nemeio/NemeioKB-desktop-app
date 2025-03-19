using System;
using System.Runtime.InteropServices;
using Nemeio.Core.DataModels.Configurator;

namespace Nemeio.Core.JsonModels
{
    public enum NemeioActionType : sbyte
    {
        Unicode = 0x01,
        Special = 0x02,
        Application = 0x03,
        Url = 0x04,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NemeioKeyboardPacket
    {
        public UInt16 Header;

        public sbyte Command;

        public short Length;

        public NemeioIndexKeystroke[] Keystrokes;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NemeioIndexKeystroke
    {
        public int Index;
    }

    public class NemeioKeystroke
    {
        public NemeioIndexKeystroke IndexKeystroke { get; set; }

        public Key Key { get; set; }
    }
}
