using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems.Hid;
using Nemeio.Platform.Windows;
using Nemeio.Platform.Windows.Layouts;
using Nemeio.Windows.Win32;

namespace Nemeio.Platform.Hid.Windows.Keyboards
{
    public class WinSystemHidAdapter : ISystemHidAdapter
    {
        private WinOsLayoutId _currentSystemLayout;

        private readonly ILogger _logger;
        private readonly object _keysLock = new object();
        private readonly List<string> _keysPressed = new List<string>();
        private readonly Dictionary<ushort, char> _vKeys = new Dictionary<ushort, char>();
        private readonly VirtualKeyBuilder _virtualKeyBuilder;
        private readonly WinOsLayoutIdBuilder _osLayoutIdBuilder;

        public WinSystemHidAdapter(ILoggerFactory loggerFactory, WinOsLayoutIdBuilder osLayoutIdBuilder)
        {
            _logger = loggerFactory.CreateLogger<WinSystemHidAdapter>();
            _virtualKeyBuilder = new VirtualKeyBuilder();
            _osLayoutIdBuilder = osLayoutIdBuilder ?? throw new ArgumentNullException(nameof(osLayoutIdBuilder));
        }

        ~WinSystemHidAdapter() 
        {
            ReleaseKeys();
        }

        public void Init() => LoadKeys();

        public void LoadKeys()
        {
            lock (_keysLock)
            {
                _vKeys.Clear();

                for (int i = 0; i <= 255; i++)
                {
                    var charForVk = WinUser32.ConvertVirtualKeyToChar((ushort)i);

                    _vKeys.Add((ushort)i, char.ToLower(charForVk));
                }

                _logger.LogInformation("Reloaded VKeys");
            }
        }

        public void ExecuteKeys(IList<SystemHidKey> keys)
        {
            _virtualKeyBuilder.Reset(_currentSystemLayout.Handle);

            var keyToRelease = _keysPressed.Except(keys.Select(x => x.Data)).ToArray();
            var keyToPress = keys;

            ReleaseKeys(keyToRelease);

            for (int i = 0; i <= keyToPress.Count - 1; i++)
            {
                var currKey = keyToPress[i];
                var key = currKey.Data;

                if (WinKeyboardConstants.IsSpecialKey(key))
                {
                    if (!WinKeyboardConstants.IsModifierKey(key))
                    {
                        if (_keysPressed.Contains(key) && currKey.Repeat)
                        {
                            _virtualKeyBuilder.AddKey(key, true);
                        }
                        else if (!_keysPressed.Contains(key))
                        {
                            _virtualKeyBuilder.AddKey(key, true);

                            _keysPressed.Add(key);
                        }
                        else
                        {
                            _logger.LogTrace($"Ignore key <{key}>");
                        }
                    }
                    else
                    {
                        if (!_keysPressed.Contains(key))
                        {
                            _keysPressed.Add(key);
                        }

                        _virtualKeyBuilder.AddKey(key, true);
                    }
                }
                else
                {
                    for (int j = 0; j <= key.Length - 1; j++)
                    {
                        var ch = key[j];

                        if (!_keysPressed.Contains(ch.ToString()))
                        {
                            _keysPressed.Add(ch.ToString());
                        }

                        if (currKey.Repeat)
                        {
                            var vk = GetVkeyForChar(ch);

                            _virtualKeyBuilder.AddVirtualKey(ch, vk);
                        }
                    }
                }
            }

            var inputsList = _virtualKeyBuilder.Build();
            if (inputsList != null && inputsList.Count > 0)
            {
                SendInputToWindows(inputsList);
            }
        }

        public void ReleaseKeys() => ReleaseEachKeys();

        public void SystemLayoutChanged(OsLayoutId layoutId)
        {
            _currentSystemLayout = _osLayoutIdBuilder.Parse(layoutId);

            LoadKeys();
        }

        #region Tools

        private void SendInputToWindows(IList<WinUser32.INPUT> inputsList) => WinUser32.SendInputs(inputsList);

        private WinUser32.INPUT GetReleaseKey(char key, IntPtr currentKbLayout)
        {
            var vk = GetVkeyForChar(key);

            return vk != default(ushort) ? WinUser32.NewUpInput(vk, currentKbLayout) : WinUser32.NewUnicodeUpInput(key);
        }

        private ushort GetVkeyForChar(char character)
        {
            lock (_keysLock)
            {
                ushort result = default(ushort);
                for (ushort i = 0; i <= _vKeys.Count - 1; i++)
                {
                    var currentChar = _vKeys[i];
                    if (currentChar == character)
                    {
                        result = i;
                        break;
                    }

                }
                return result;
            }
        }

        private void ReleaseEachKeys() => ReleaseKeys(_keysPressed.ToArray());

        private void ReleaseKeys(string[] keys)
        {
            var inputUpList = new List<WinUser32.INPUT>();

            for (int i = 0; i <= keys.Count() - 1; i++)
            {
                var inpt = keys[i];
                if (WinKeyboardConstants.IsSpecialKey(inpt))
                {
                    var newUpInput = VirtualKeyBuilder.CreateInput(inpt, false, _currentSystemLayout.Handle);
                    inputUpList.Add(newUpInput);
                }
                else
                {
                    foreach (var ch in inpt)
                    {
                        var releaseKey = GetReleaseKey(ch, _currentSystemLayout.Handle);
                        inputUpList.Add(releaseKey);
                    }
                }

                _keysPressed.Remove(inpt);
            }

            if (inputUpList?.Count > 0)
            {
                SendInputToWindows(inputUpList);
            }
        }

        #endregion
    }
}
