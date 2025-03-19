using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Nemeio.Core.Errors;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;
using Nemeio.Windows.Win32;

namespace Nemeio.Platform.Windows.Layouts.Systems
{
    public class WinSystemActiveLayoutAdapter : ISystemActiveLayoutAdapter
    {
        private const string ApplicationFrameHostProcessName = "ApplicationFrameHost";

        private readonly ILogger _logger;
        private readonly IErrorManager _errorManager;
        private readonly WinOsLayoutIdBuilder _osLayoutIdBuilder;

        private IntPtr _applicationFrameHostSettingsWindow;

        public event EventHandler OnSystemActionLayoutChanged;

        public WinSystemActiveLayoutAdapter(ILoggerFactory loggerFactory, IErrorManager errorManager, WinOsLayoutIdBuilder osLayoutBuilder)
        {
            _logger = loggerFactory.CreateLogger<WinSystemActiveLayoutAdapter>();
            _errorManager = errorManager ?? throw new ArgumentNullException(nameof(errorManager));
            _osLayoutIdBuilder = osLayoutBuilder ?? throw new ArgumentNullException(nameof(osLayoutBuilder));
        }

        public OsLayoutId GetCurrentSystemLayoutId()
        {
            var windowHandle = WinUser32.GetForegroundWindow();
            var processId = WinUser32.GetWindowProcessId(windowHandle);

            if (IsApplicationFrameHostProcess(processId))
            {
                _applicationFrameHostSettingsWindow = IntPtr.Zero;

                WorkOutApplicationFrameHostSettingsWindow(processId);

                if (_applicationFrameHostSettingsWindow == IntPtr.Zero)
                {
                    _logger.LogInformation($"ApplicationFrameHost encountered and not resolved ");
                }
                else
                {
                    windowHandle = _applicationFrameHostSettingsWindow;
                }
            }

            var dwThread = WinUser32.GetWindowThreadProcessId(windowHandle, IntPtr.Zero);
            var systemId = WinUser32.GetKeyboardLayout(dwThread);

            if (systemId == IntPtr.Zero)
            {
                var sysId = int.Parse(GetDefaultSystemLayoutId().Id);
                systemId = new IntPtr(sysId);
            }
            return _osLayoutIdBuilder.Build(systemId);
        }

        public OsLayoutId GetDefaultSystemLayoutId()
        {
            var osLayoutId = _osLayoutIdBuilder.Build(InputLanguage.DefaultInputLanguage.Handle);

            return osLayoutId;
        }

        private WinOsLayoutId GetSystemLayoutId()
        {
            return GetCurrentSystemLayoutId() as WinOsLayoutId;
        }

        private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.Locale)
            {
                OnSystemActionLayoutChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        private bool IsApplicationFrameHostProcess(int processId)
        {
            if (processId > 1)
            {
                try
                {
                    var process = Process.GetProcessById(processId);
                    if (process != null && process.ProcessName != null)
                    {
                        return process.ProcessName.Equals(ApplicationFrameHostProcessName);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, _errorManager.GetFullErrorMessage(ErrorCode.WindowsGetProcessIdFailed));
                }
            }
            return false;
        }

        private void WorkOutApplicationFrameHostSettingsWindow(int processId)
        {
            var process = Process.GetProcessById(processId);
            if (process != null && process.MainWindowHandle != IntPtr.Zero)
            {
                WinUser32.EnumChildWindows(process.MainWindowHandle, ChildWindowCallback, IntPtr.Zero);
            }
        }

        private bool ChildWindowCallback(IntPtr hwnd, IntPtr lparam)
        {
            try
            {
                int processId;

                WinUser32.GetWindowThreadProcessId(hwnd, out processId);

                var process = Process.GetProcessById(processId);
                if (process.ProcessName != "ApplicationFrameHost")
                {
                    _applicationFrameHostSettingsWindow = hwnd;
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"WinLayoutWatcher.ChildWindowCallback exception found");
                _logger.LogError(exception, $"WinLayoutWatcher.ChildWindowCallback exception found {_errorManager.GetFullErrorMessage(ErrorCode.WindowsGetProcessIdFailed)}");
            }

            return true;
        }
    }
}
