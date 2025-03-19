namespace Nemeio.Core.Systems.Applications
{
    public class Application
    {
        private string _applicationPath;

        public string Name { get; set; }
        public string WindowTitle { get; set; }
        public int ProcessId { get; set; }
        public string ProcessName { get; set; }
        public bool IsAdministrator { get; set; }
        public string ApplicationPath
        {
            get => _applicationPath;
            set => _applicationPath = CoreHelpers.SanitizePath(value);
        }

        public override string ToString() => $"<IsAdministrator={IsAdministrator}>, <ApplicationPath={ApplicationPath}>, <Name={Name}>, <WindowTitle={WindowTitle}>, <ProcessName={ProcessName}>, <ProcessId={ProcessId}>";
    }
}
