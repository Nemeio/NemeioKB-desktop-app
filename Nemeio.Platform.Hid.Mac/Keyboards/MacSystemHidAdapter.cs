using System;
using System.Collections.Generic;
using System.Linq;
using AppKit;
using CoreGraphics;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems.Hid;
using Nemeio.Mac.Native;
using Nemeio.Platform.Mac;
using Nemeio.Platform.Mac.Utils;

namespace Nemeio.Platform.Hid.Mac.Keyboards
{
    public class MacSystemHidAdapter : ISystemHidAdapter, IDisposable
    {
        private const int VirtualKeyLength = 255;
        private const ushort NotFoundVirtualKey = 65535;

        private bool _disposed = false;

        private readonly ILogger _logger;

        private readonly object _keysLock = new object();
        private readonly Dictionary<ushort, char> _vKeys = new Dictionary<ushort, char>();
        private readonly List<string> _keysPressed = new List<string>();

        private readonly KeyboardInputFactory _inputFactory;

        public string LayoutName { get; set; }

        public MacSystemHidAdapter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MacSystemHidAdapter>();
            _inputFactory = new KeyboardInputFactory();
        }

        ~MacSystemHidAdapter()
        {
            Dispose(false);
        }

        public void Init() => LoadKeys();

        public void LoadKeys()
        {
            lock (_keysLock)
            {
                _vKeys.Clear();

                for (ushort i = 0; i <= VirtualKeyLength; i++)
                {
                    var stringForVk = ExtendedTools.CreateStringForKeyWithModifiers(i, LayoutName, false, false, false);
                    if (stringForVk != null && stringForVk.Length > 0)
                    {
                        var charForVk = stringForVk[0];

                        _vKeys.Add(i, char.ToLower(charForVk));
                    }
                }

                _logger.LogInformation("Reloaded VKeys");
            }
        }

        public void SystemLayoutChanged(OsLayoutId layoutId)
        {
            DispatchQueueUtils.DispatchSyncOnMainQueueIfNeeded(() =>
            {
                var inputSource = string.Empty;
                var inputContext = new NSTextInputContext();
                LayoutName = inputContext.SelectedKeyboardInputSource;
            });

            LoadKeys();
        }

        public void ExecuteKeys(IList<SystemHidKey> keys)
        {
            var pressEventList = new List<IntPtr>();
            var unpressEventList = new List<IntPtr>();

            var keyToRelease = _keysPressed.Except(keys.Select(x => x.Data)).ToArray();
            var keyToPress = keys;

            ReleaseKeys(keyToRelease);

            for (int i = 0; i < keyToPress.Count(); i++)
            {
                var currKey = keyToPress[i];
                var key = currKey.Data;

                if (MacKeyboardConstants.IsSpecialKey(key))
                {
                    if (!MacKeyboardConstants.IsModifierKey(key))
                    {
                        if (_keysPressed.Contains(key) && currKey.Repeat)
                        {
                            pressEventList.Add(_inputFactory.CreateNewInput(_keysPressed, key, true));
                        }
                        else if (!_keysPressed.Contains(key))
                        {
                            pressEventList.Add(_inputFactory.CreateNewInput(_keysPressed, key, true));

                            _keysPressed.Add(key);
                        }
                    }
                    else
                    {
                        if (!_keysPressed.Contains(key))
                        {
                            _keysPressed.Add(key);
                        }

                        pressEventList.Add(_inputFactory.CreateNewInput(_keysPressed, key, true));
                    }
                }
                else
                {
                    for (int j = 0; j < key.Length; j++)
                    {
                        var ch = key[j];
                        if (!_keysPressed.Contains(ch.ToString()))
                        {
                            _keysPressed.Add(ch.ToString());
                        }

                        if (currKey.Repeat)
                        {
                            var keyCode = GetKeyCodeForChar(ch);

                            if (keyCode != NotFoundVirtualKey)
                            {
                                pressEventList.Add(_inputFactory.CreateNewInput(_keysPressed, (CGKeyCode)keyCode, true));
                            }
                            else
                            {
                                var keyEvent = _inputFactory.CreateNewInput(_keysPressed, CGKeyCode.kVK_None, true);
                                QuartzEvent.CGEventKeyboardSetUnicodeString(keyEvent, 1, new char[1] { ch });

                                pressEventList.Add(keyEvent);
                            }
                        }
                    }
                }
            }

            var inputsList = pressEventList.Concat(unpressEventList).ToList();
            var finalInputList = new List<IntPtr>();

            foreach (var input in inputsList)
            {
                if (input != IntPtr.Zero)
                {
                    finalInputList.Add(input);
                }
            }

            if (finalInputList.Any())
            {
                _logger.LogInformation($"Send key to OSX {finalInputList.Count}");

                SendInputToOSX(finalInputList);
            }
        }

        public void ReleaseKeys() => ReleaseEachKeys();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                ReleaseEachKeys();
            }

            _disposed = true;
        }

        #region Tools

        private ushort GetKeyCodeForChar(char character)
        {
            lock (_keysLock)
            {
                ushort result = NotFoundVirtualKey;
                for (ushort i = 0; i < _vKeys.Count; i++)
                {
                    if (_vKeys.ContainsKey(i))
                    {
                        var currentChar = _vKeys[i];
                        if (currentChar == character)
                        {
                            result = i;
                            break;
                        }
                    }
                }
                return result;
            }
        }

        private IntPtr GetReleaseKey(char key)
        {
            var keyCode = GetKeyCodeForChar(key);

            var src = new CGEventSource(CGEventSourceStateID.HidSystem);
            var unpressEvent = QuartzEvent.CGEventCreateKeyboardEvent(src.Handle, (CGKeyCode)keyCode, false);

            if (keyCode == NotFoundVirtualKey)
            {
                QuartzEvent.CGEventKeyboardSetUnicodeString(unpressEvent, 1, new char[1] { key });
            }

            return unpressEvent;
        }

        private void ReleaseEachKeys()
        {
            ReleaseKeys(_keysPressed.ToArray());
            _keysPressed.Clear();
        }

        private void ReleaseKeys(string[] keys)
        {
            var events = new List<IntPtr>();

            for (int i = 0; i < keys.Count(); i++)
            {
                var inpt = keys[i];

                if (MacKeyboardConstants.IsSpecialKey(inpt) || MacKeyboardConstants.IsModifierKey(inpt))
                {
                    _logger.LogInformation($"Release Key Special : {inpt}");

                    events.Add(_inputFactory.CreateNewInput(_keysPressed, inpt, false));
                }
                else
                {
                    foreach (var ch in inpt)
                    {
                        _logger.LogInformation($"Release Key : {inpt}");
                        events.Add(
                            GetReleaseKey(ch)
                        );
                    }
                }
                _keysPressed.Remove(inpt);
            }

            var finalEvents = new List<IntPtr>();
            foreach (var evt in events)
            {
                if (evt != IntPtr.Zero)
                {
                    finalEvents.Add(evt);
                }
            }

            if (finalEvents.Count > 0)
            {
                _logger.LogInformation($"Did release keystroke {finalEvents.Count()}");

                SendInputToOSX(finalEvents);
            }
        }

        private void SendInputToOSX(List<IntPtr> actions)
        {
            DispatchQueueUtils.DispatchAsyncOnMainQueueIfNeeded(() =>
            {
                foreach (var action in actions)
                {
                    QuartzEvent.CGEventPost(CGEventTapLocation.HID, action);
                }

                foreach (var action in actions)
                {
                    CF.CFRelease(action);
                }
            });
        }

        #endregion
    }
}
