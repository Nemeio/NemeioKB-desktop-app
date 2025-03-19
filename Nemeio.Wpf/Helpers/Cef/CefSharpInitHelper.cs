using System;
using System.Runtime.CompilerServices;
using CefSharp.Wpf;
using Microsoft.Extensions.Logging;

namespace Nemeio.Wpf.Helpers.Cef
{
    internal static class CefSharpInitHelper
    {
        internal static void Init(ILogger logger)
        {
            InitializeCefSharp(logger);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void InitializeCefSharp(ILogger logger)
        {
            try
            {
                var settings = new CefSettings
                {
                    MultiThreadedMessageLoop = true,
                    ExternalMessagePump = false,
                    IgnoreCertificateErrors = true,
                };
                settings.CefCommandLineArgs.Add("disable-gpu", "1");
                settings.CefCommandLineArgs.Add("disable-gpu-vsync", "1");

                CefSharp.Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Init CEFSharp failed");

                throw;
            }
        }
    }
}
