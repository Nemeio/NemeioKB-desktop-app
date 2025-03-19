using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using Microsoft.Extensions.Logging;
using MvvmCross.Platform;
using Nemeio.Core.Applications;
using Nemeio.Core.Services;
using Nemeio.Presentation;

namespace Nemeio.Wpf
{
    public partial class App
    {
        private static readonly string UniqueAppInstanceMutexName = "Nemeio";
        private static readonly string RestartAsAdminArgument = "restartAsAdmin";

        private Mutex _mutex;

        private ILoggerFactory _loggerFactory;
        private ILogger _logger;

        public static bool StopRequired { get; internal set; }

        public App()
        {
            InitLogger();

            AppDomain.CurrentDomain.UnhandledException += LogUnhandledException;
            Helpers.Cef.CefSharpInitHelper.Init(_logger);

            LoadMvxAssemblyResources();
        }

        private void InitLogger()
        {
            _loggerFactory = Logger.GetLoggerFactory();
            _logger = _loggerFactory.CreateLogger<App>();
        }

        private void LogUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger?.LogError((Exception)e.ExceptionObject, "Unhandled exception");
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            //  First of all we check if we must stop app or not
            //  Only one instance of Nemeio is allowed

            if (!IsUniqueInstance())
            {

                if (!e.Args.Contains(RestartAsAdminArgument))
                {
                    _logger.LogInformation("Another instance is already running. Closing Application...");
                    StopRequired = true;
                    Environment.Exit(-1);
                }
            }

            Exit += AppExit;
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;

            new WpfSetup(Dispatcher, _loggerFactory).Initialize();

            Presentation.Application.ApplicationStarted();

            base.OnStartup(e);
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.LogError(e.Exception, "UnhandledException");
        }

        private bool IsUniqueInstance()
        {
            _mutex = new Mutex(true, UniqueAppInstanceMutexName, out bool createdNew);
            return createdNew;
        }

        private void AppExit(object sender, ExitEventArgs e)
        {
            _logger.LogInformation("App exited");
            Mvx.Resolve<IApplicationController>().ShutDown();
            Mvx.Resolve<INemeioHttpService>().StopListeningToRequestsAsync();
            _mutex?.ReleaseMutex();
        }

        public void RestartAsAdmin()
        {
            try
            {
                // Start the same process elevated as administrator.
                ProcessStartInfo process = new ProcessStartInfo(System.Reflection.Assembly.GetEntryAssembly().Location);
                process.UseShellExecute = true;
                process.Verb = "runas";
                process.Arguments = RestartAsAdminArgument;

                Process.Start(process);

                Mvx.Resolve<INemeioHttpService>().StopListeningToRequestsAsync();
                Mvx.Resolve<IApplicationController>().ShutDown();

                // If process start correctly, shutdown the current appplication.
                Current?.Shutdown();
            }
            catch (Win32Exception)
            {
                // Do nothing if user refuse to grant privilege.
                _logger.LogInformation("RestartAsAdmin-user refuse to grant privilege.");
            }
        }

        private void LoadMvxAssemblyResources()
        {
            for (var i = 0; ; i++)
            {
                var key = "MvxAssemblyImport" + i;
                var data = TryFindResource(key);
                if (data == null) { return; }
            }
        }
    }
}
