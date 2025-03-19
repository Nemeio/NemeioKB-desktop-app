using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Nemeio.Core;
using Nemeio.Core.Errors;
using Nemeio.Core.Services;
using Nemeio.Core.Systems.Applications;
using Nemeio.Core.Tools.Dispatcher;
using Nemeio.Windows.Win32;

namespace Nemeio.Platform.Windows.Applications
{
    public class WinSystemForegroundApplicationAdapter : ISystemForegroundApplicationAdapter
    {
        private const uint WinEventOutOfContext = 0;
        private const uint EventSystemForeground = 0x0003;
        private const uint EventSystemMinimizeEnd = 0x0017;

        private readonly ILogger _logger;
        private readonly IErrorManager _errorManager;
        private readonly IDispatcher _mainThreadDispatcher;

        private IntPtr _eventHook;
        private DateTime _latestTrace;
        private WinUser32.WinEventDelegate _winEventProc;
        private int _lastProcessId;

        public event EventHandler<ApplicationChangedEventArgs> OnApplicationChanged;

        public WinSystemForegroundApplicationAdapter(ILoggerFactory loggerFactory, IErrorManager errorManager, IApplicationService applicationService)
        {
            _logger = loggerFactory.CreateLogger<WinSystemForegroundApplicationAdapter>();
            _errorManager = errorManager ?? throw new ArgumentNullException(nameof(errorManager));
            _mainThreadDispatcher = applicationService.GetMainThreadDispatcher();
        }

        public void Start()
        {
            _mainThreadDispatcher.Invoke(() =>
            {
                _winEventProc = new WinUser32.WinEventDelegate(WinEventProc);
                _eventHook = WinUser32.SetWinEventHook(
                    EventSystemForeground,
                    EventSystemMinimizeEnd,
                    IntPtr.Zero,
                    _winEventProc,
                    0,
                    0,
                    WinEventOutOfContext
                );
            });

            var hwnd = WinUser32.GetForegroundWindow();
            ProcessApplicationForegroundChanged(hwnd);
        }

        public void Stop()
        {
            _mainThreadDispatcher.Invoke(() =>
            {
                //  Unhook must be on the same thread of hook method
                //  In this case MainThread
                var unhookSuccess = WinUser32.UnhookWinEvent(_eventHook);

                _logger.LogInformation($"WinSystemForegroundApplicationAdapter UnhookWinEvent {unhookSuccess}");
            });
        }

        #region Tools

        private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (eventType == EventSystemForeground ||
                eventType == EventSystemMinimizeEnd)
            {
                ProcessApplicationForegroundChanged(hwnd);
            }
        }

        private void ProcessApplicationForegroundChanged(IntPtr hwnd)
        {
            try
            {
                var windowTitle = WinUser32.GetWindowTitle(hwnd);
                var processId = WinUser32.GetWindowProcessId(hwnd);

                var detectedApplication = new Application
                {
                    WindowTitle = windowTitle,
                    ProcessId = processId
                };

                if (detectedApplication.ProcessId <= 0)
                {
                    return;
                }

                var process = Process.GetProcessById(detectedApplication.ProcessId);

#if DEBUG
                if (process.ProcessName == "devenv")
                {
                    return;
                }

                if (IsNemeioApplication(process))
                {
                    //  We need to avoid to save Nemeio's admin page as last process

                    return;
                }
#endif

                _lastProcessId = detectedApplication.ProcessId;

                if (IsNemeioApplication(process))
                {
                    //  Special case
                    //  We won't manage Nemeio has classic application (e.g. manage Admin mode, ...)
                    //  We only want to detect configrator window

                    RetrieveLastProcessInformation(detectedApplication, process);
                    RaiseApplicationChanged(detectedApplication);

                    return;
                }

                detectedApplication.ProcessName = process.ProcessName;

                var currentProcess = Process.GetCurrentProcess();

                if (!ProcessHelper.IsRunningAsAdministrator(detectedApplication.ProcessId) || ProcessHelper.IsRunningAsAdministrator(currentProcess.Id))
                {
                    RetrieveLastProcessInformation(detectedApplication, process);
                    detectedApplication.IsAdministrator = false;
                }
                else
                {
                    TimedTrace($"Administrator Foreground app - PID : <{detectedApplication.ProcessId}> Process Name : <{process.ProcessName}>");
                    detectedApplication.IsAdministrator = true;
                }

                RaiseApplicationChanged(detectedApplication);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    _errorManager.GetFullErrorMessage(ErrorCode.WindowsGetProcessIdFailed)
                );
            }
        }

        private void RetrieveLastProcessInformation(Application app, Process process)
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(process.MainModule.FileName);
            if (versionInfo.FileDescription != null)
            {
                app.Name = versionInfo.FileDescription.ToLower();
            }

            app.ApplicationPath = process.MainModule.FileName;
        }

        private bool IsNemeioApplication(Process process) => process.ProcessName.Equals(NemeioConstants.AppName, StringComparison.OrdinalIgnoreCase);

        private void TimedTrace(string message)
        {
            // trace on regular time
            if ((DateTime.Now - _latestTrace).TotalSeconds < 10)
            {
                return;
            }

            _latestTrace = DateTime.Now;
            _logger.LogInformation(message);
        }

        private void RaiseApplicationChanged(Application app) => OnApplicationChanged?.Invoke(this, new ApplicationChangedEventArgs(app));

        #endregion
    }
}
