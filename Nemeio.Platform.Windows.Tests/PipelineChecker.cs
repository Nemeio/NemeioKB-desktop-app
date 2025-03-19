using System;

namespace Nemeio.Platform.Windows.Tests
{
    public static class PipelineChecker
    {
        private const string PipelineEnvironnementVariableName = "Pipeline";
        private const string PipelineIsEnableValue = "TRUE";

        public static  bool RunningOnPipeline()
        {
            var isPipeline = Environment.GetEnvironmentVariable(PipelineEnvironnementVariableName);
            if (!string.IsNullOrEmpty(isPipeline))
            {
                if (isPipeline.Equals(PipelineIsEnableValue))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
