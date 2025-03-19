using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Nemeio.Windows.Win32.Inputs;

namespace Nemeio.Windows.Win32
{
    public static class WinUser32
    {
        #region Constantes

        private const int HWND_BROADCAST = 0xffff;
        private const int WM_INPUTLANGCHANGEREQUEST = 0x0050;

        const int INPUT_MOUSE = 0;
        const int INPUT_KEYBOARD = 1;
        const int INPUT_HARDWARE = 2;
        public const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        const uint KEYEVENTF_KEYUP = 0x0002;
        const uint KEYEVENTF_KEYDOWN = 0;
        const uint KEYEVENTF_UNICODE = 0x0004;
        const uint KEYEVENTF_SCANCODE = 0x0008;
        const uint XBUTTON1 = 0x0001;
        const uint XBUTTON2 = 0x0002;
        const uint MOUSEEVENTF_MOVE = 0x0001;
        const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        const uint MOUSEEVENTF_LEFTUP = 0x0004;
        const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
        const uint MOUSEEVENTF_XDOWN = 0x0080;
        const uint MOUSEEVENTF_XUP = 0x0100;
        const uint MOUSEEVENTF_WHEEL = 0x0800;
        const uint MOUSEEVENTF_VIRTUALDESK = 0x4000;
        const uint MOUSEEVENTF_ABSOLUTE = 0x8000;

        const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        const int APPCOMMAND_VOLUME_UP = 0xA0000;
        const int APPCOMMAND_VOLUME_DOWN = 0x90000;
        const int WM_APPCOMMAND = 0x319;

        const int WM_PAINT = 0xF;
        private const int SCANCODE_EXTENDED_FLAG = 0xe000;

        public enum KLF
        {
            REORDER = 0x00000008,
            RESET = 0x40000000,
            SETFORPROCESS = 0x00000100,
            SHIFTLOCK = 0x00010000
        }

        #endregion

        #region Structures

        [StructLayout(LayoutKind.Sequential)]
        public struct GUITHREADINFO
        {
            public int cbSize;
            public int flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCaret;
            public RECT rectCaret;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DevBroadcastDeviceinterface
        {
            internal int Size;

            internal int DeviceType;

            internal int Reserved;

            internal Guid ClassGuid;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
            internal string Name;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct RawInputDeviceList
        {
            internal IntPtr hDevice;

            internal RawInputDeviceType dwType;
        }

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto)]
        public struct RawDeviceInfo
        {
            [FieldOffset(0)]
            internal int Size;

            [FieldOffset(4)]
            internal int Type;

            [FieldOffset(8)]
            internal DeviceInfoMouse MouseInfo;

            [FieldOffset(8)]
            internal DeviceInfoKeyboard KeyboardInfo;

            [FieldOffset(8)]
            internal DeviceInfoHID HIDInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            public int type;

            public InputUnion union;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct InputUnion
        {
            [FieldOffset(0)]
            public MOUSEINPUT mouseInput;

            [FieldOffset(0)]
            public KEYBDINPUT keyboardInput;

            [FieldOffset(0)]
            public HARDWAREINPUT hardwareInput;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            /*Virtual Key code.  Must be from 1-254.  If the dwFlags member specifies KEYEVENTF_UNICODE, wVk must be 0.*/
            public ushort wVk;

            /*A hardware scan code for the key. If dwFlags specifies KEYEVENTF_UNICODE, wScan specifies a Unicode character which is to be sent to the foreground application.*/
            public ushort wScan;

            /*Specifies various aspects of a keystroke.  See the KEYEVENTF_ constants for more information.*/
            public uint flags;

            /*The time stamp for the event, in milliseconds. If this parameter is zero, the system will provide its own time stamp.*/
            public uint time;

            /*An additional value associated with the keystroke. Use the GetMessageExtraInfo function to obtain this information.*/
            public IntPtr dwExtraInfo;

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NOTIFYICONIDENTIFIER
        {
            public uint cbSize;
            public IntPtr hWnd;
            public uint uID;
            public Guid guidItem;
        }
        #endregion

        #region Enum

        public enum RawInputDeviceInfoCommand : uint
        {
            PreparsedData = 0x20000005,
            DeviceName = 0x20000007,
            DeviceInfo = 0x2000000b,
        }

        public enum RawInputDeviceType : uint
        {
            Mouse = 0,
            Keyboard = 1,
            HumanInterfaceDevice = 2
        }

        public enum MAPVK : uint
        {
            MAPVK_VK_TO_VSC = 0,
            MAPVK_VSC_TO_VK = 1,
            MAPVK_VK_TO_CHAR = 2,
            MAPVK_VSC_TO_VK_EX = 3,
            MAPVK_VK_TO_VSC_EX = 4
        }

        public enum SPI : uint
        {
            GETKEYBOARDDELAY = 0x0016,
            GETKEYBOARDSPEED = 0x000A
        }

        #endregion

        #region InputDevice

        [StructLayout(LayoutKind.Sequential)]
        public struct DeviceInfoMouse
        {
            public uint ID;
            public uint NumberOfButtons;
            public uint SampleRate;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DeviceInfoKeyboard
        {
            public uint Type;
            public uint SubType;
            public uint KeyboardMode;
            public uint NumberOfFunctionKeys;
            public uint NumberOfIndicators;
            public uint NumberOfKeysTotal;
            public bool IsUSBKeboard
            {
                get
                {
                    return this.Type == 81; // http://msdn.microsoft.com/en-us/library/windows/desktop/ms724336%28v=vs.85%29.aspx
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DeviceInfoHID
        {
            public uint VendorID;
            public uint ProductID;
            public uint VersionNumber;
            public ushort UsagePage;
            public ushort Usage;
        }

        #endregion

        #region Delegates

        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        public delegate bool WindowEnumProc(IntPtr hwnd, IntPtr lparam);

        #endregion

        #region API

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, uint flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool UnregisterDeviceNotification(IntPtr handle);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint GetRawInputDeviceList([In, Out] RawInputDeviceList[] RawInputDeviceList, ref uint numDevices, uint size);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetRawInputDeviceInfo(IntPtr hDevice, RawInputDeviceInfoCommand uiCommand, IntPtr data, ref uint size);

        [DllImport("user32.dll")]
        static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        public static extern short VkKeyScan(char ch);

        [DllImport("user32.dll")]
        public static extern bool LockWorkStation();

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        [DllImport("user32.DLL")]
        public static extern int SendMessageW(IntPtr hwnd, int msg, int character, IntPtr lpsText);

        [DllImport("user32", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowExA(IntPtr hWnd1, int hWnd2, string lpsz1, string lpsz2);

        [DllImport("user32", EntryPoint = "MapVirtualKeyA")]
        public static extern uint MapVirtualKeyA(uint code, uint mapType);

        [DllImport("user32", EntryPoint = "MapVirtualKeyExW")]
        public static extern uint MapVirtualKeyExW(uint code, uint mapType, IntPtr kLayout);

        [DllImport("user32", EntryPoint = "ToAsciiEx")]
        public static extern uint ToAsciiEx(uint vk, uint sc, out byte[] state, out ushort lpChar, uint flags, IntPtr kLayout);

        [DllImport("user32", EntryPoint = "GetKeyboardState")]
        public static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hwnd, IntPtr proccess);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll")]
        public static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll")]
        public static extern int GetKeyboardLayoutList(int nBuff, IntPtr[] lpList);

        [DllImport("user32.dll")]
        public static extern int GetKeyboardLayoutNameA(IntPtr[] pwszKLID);

        [DllImport("user32.dll")]
        public static extern int GetKeyboardLayoutNameW(IntPtr[] pwszKLID);

        [DllImport("user32.dll")]
        public static extern int LoadKeyboardLayoutA(string inputLocaleId, uint flags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetGUIThreadInfo(int hTreadID, ref GUITHREADINFO lpgui);

        [DllImport("user32.dll", EntryPoint = "ActivateKeyboardLayout")]
        public static extern IntPtr ActivateKeyboardLayout(IntPtr hkl, uint flags);

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfoA")]
        public static extern bool SystemParametersInfoA(uint uiAction, uint uiParam, ref int pvParam, uint fWinIni);

        [DllImport("user32.dll")]
        public static extern int ToUnicode(uint virtualKeyCode, uint scanCode,
            byte[] keyboardState,
            [Out, MarshalAs(UnmanagedType.LPWStr, SizeConst = 64)]
            StringBuilder receivingBuffer,
            int bufferSize, uint flags);

        [DllImport("user32.dll")]
        public static extern int ToUnicodeEx(
            uint virtualKeyCode,
            uint scanCode,
            byte[] keyboardState,
            [Out, MarshalAs(UnmanagedType.LPWStr, SizeConst = 64)]
            StringBuilder receivingBuffer,
            int bufferSize,
            uint flags,
            IntPtr dwhkl
        );

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        public static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc callback, IntPtr lParam);

        [DllImport("shell32.dll", SetLastError = true)]
        public static extern int Shell_NotifyIconGetRect([In] ref NOTIFYICONIDENTIFIER identifier, [Out] out RECT iconLocation);

        [DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

        #endregion

        #region Methods

        public static int GetKeyboardDelay()
        {
            return GetKeyboardValue((uint)SPI.GETKEYBOARDDELAY);
        }

        public static int GetKeyboardSpeed()
        {
            return GetKeyboardValue((uint)SPI.GETKEYBOARDSPEED);
        }

        static int GetKeyboardValue(uint valueName)
        {
            int rep = 0;

            WinUser32.SystemParametersInfoA(valueName, 0, ref rep, 0);

            return rep;
        }

        public static char ConvertVirtualKeyToChar(uint vk)
        {
            return (char)MapVirtualKeyA(vk, (uint)MAPVK.MAPVK_VK_TO_CHAR);
        }

        public static INPUT NewDownInput(ushort virtualKey, IntPtr kLayout)
        {
            return NewInput(virtualKey, 0, KEYEVENTF_KEYDOWN, kLayout);
        }

        public static INPUT NewUpInput(ushort virtualKey, IntPtr kLayout)
        {
            return NewInput(virtualKey, 0, KEYEVENTF_KEYUP, kLayout);
        }

        public static INPUT NewUnicodeDownInput(char unicodeChar)
        {
            return NewInput(0, unicodeChar, KEYEVENTF_KEYDOWN, IntPtr.Zero);
        }

        public static INPUT NewUnicodeUpInput(char unicodeChar)
        {
            return NewInput(0, unicodeChar, KEYEVENTF_KEYUP, IntPtr.Zero);
        }

        public static INPUT NewInput(ushort virtualKey, ushort unicode, uint state, IntPtr kLayout)
        {
            if (unicode != 0)
            {
                INPUT unicodeInpt = new INPUT
                {
                    // Need a keyboard event.
                    type = INPUT_KEYBOARD,
                    union = new InputUnion
                    {
                        // KEYBDINPUT will contain all the information for a single keyboard event
                        // (more precisely, for a single key-down or key-up).
                        keyboardInput = new KEYBDINPUT
                        {
                            wVk = 0,

                            wScan = unicode,

                            flags = KEYEVENTF_UNICODE | state,

                            dwExtraInfo = GetMessageExtraInfo(),
                        }
                    }
                };

                return unicodeInpt;
            }

            var scanCode = MapVirtualKeyExW(virtualKey, (uint)MAPVK.MAPVK_VK_TO_VSC_EX, kLayout);
            var extendedCode = (scanCode & SCANCODE_EXTENDED_FLAG) != 0;

            uint flags = KEYEVENTF_SCANCODE | state;

            if (extendedCode || IsExtended(virtualKey))
            {
                flags |= KEYEVENTF_EXTENDEDKEY;
            }

            INPUT input = new INPUT
            {
                // Need a keyboard event.
                type = INPUT_KEYBOARD,
                union = new InputUnion
                {
                    // KEYBDINPUT will contain all the information for a single keyboard event
                    // (more precisely, for a single key-down or key-up).
                    keyboardInput = new KEYBDINPUT
                    {
                        // Virtual-key code must be 0 since we are sending Unicode characters.
                        wVk = virtualKey,

                        // The Unicode character to be sent.
                        wScan = (ushort)scanCode,

                        // Indicate that we are sending a Unicode character.
                        // Also indicate key-up on the second iteration.
                        flags = flags,

                        dwExtraInfo = GetMessageExtraInfo(),
                    }
                }
            };

            return input;
        }

        public static void SendInputs(IList<INPUT> inputs)
        {
            SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(typeof(INPUT)));
        }

        public static ushort ConvertCharToVirtualKey(char ch)
        {
            short vkey = VkKeyScan(ch);

            return (ushort)vkey;
        }

        public static bool IsExtended(ushort virtualKey) => _extendedVirutalKeys.Contains(virtualKey);

        private static List<ushort> _extendedVirutalKeys = new List<ushort>()
        {
            (ushort)Keys.Left,
            (ushort)Keys.Up,
            (ushort)Keys.Right,
            (ushort)Keys.Down,
            (ushort)Keys.End,
            (ushort)Keys.Home,
            (ushort)Keys.Insert,
            (ushort)Keys.Delete,
            (ushort)Keys.NumLock
        };

        public static string GetWindowTitle(IntPtr hwnd)
        {
            var Buff = new StringBuilder(500);
            GetWindowText(hwnd, Buff, Buff.Capacity);
            return Buff.ToString();
        }

        public static int GetWindowProcessId(IntPtr hwnd)
        {
            var pid = 1;
            GetWindowThreadProcessId(hwnd, out pid);
            return pid;
        }

        public static void RequestLanguageChange(IntPtr targetedLayout)
        {
            PostMessage(new IntPtr(HWND_BROADCAST), WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, targetedLayout);
        }


        public static Microsoft.Win32.RegistryKey OpenSubKey(string key, bool writable)
        {
            return Microsoft.Win32.Registry.CurrentUser.OpenSubKey(key, writable);
        }

        #endregion
    }
}
