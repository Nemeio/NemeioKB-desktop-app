namespace Nemeio.Core.DataModels
{
    public class ProcessInformation
    {
        private string _applicationPath;

        public string WindowTitle { get; set; }
        public string ProcessName { get; set; }
        public int ProcessId { get; set; }
        public string ApplicationPath
        {
            get
            {
                return _applicationPath;
            }

            set
            {
                _applicationPath = CoreHelpers.SanitizePath(value);
            }
        }

        public bool IsAdministrator { get; set; }
        public string ApplicationName { get; set; }

        public ProcessInformation() { }

        public override string ToString()
        {
            return $"<IsAdministrator={IsAdministrator}>, <ApplicationPath={ApplicationPath}>, <ApplicationName={ApplicationName}>, <WindowTitle={WindowTitle}>, <ProcessName={ProcessName}>, <ProcessId={ProcessId}>";
        }
    }
}
