using System;
using Microsoft.Extensions.Logging;
using MvvmCross.Platform.Platform;

namespace Nemeio.Presentation
{
    public class MvxTraceProxy : IMvxTrace
    {
        private readonly ILogger _logger;

        public MvxTraceProxy(ILoggerFactory loggerFactory)
        {
            #pragma warning disable CS0618
            _logger = loggerFactory.CreateLogger<MvxTrace>();
            #pragma warning restore CS0618
        }

        public void Trace(MvxTraceLevel level, string tag, Func<string> message) => Log(level, $"{tag}:{level}:{message()}");

        public void Trace(MvxTraceLevel level, string tag, string message) => Log(level, $"{tag}:{level}:{message}");

        public void Trace(MvxTraceLevel level, string tag, string message, params object[] args)
        {
            try
            {
                Log(level, string.Format($"{tag}:{level}:{message}", args));
            }
            catch (FormatException)
            {
                Trace(MvxTraceLevel.Error, tag, $"Exception during trace of {level} {message}");
            }
        }

        private void Log(MvxTraceLevel level, string message)
        {
            switch (level)
            {
                case MvxTraceLevel.Diagnostic:
                    _logger.LogInformation(message);
                    break;
                case MvxTraceLevel.Warning:
                    _logger.LogWarning(message);
                    break;
                case MvxTraceLevel.Error:
                    _logger.LogError(message);
                    break;
            }
        }
    }
}
