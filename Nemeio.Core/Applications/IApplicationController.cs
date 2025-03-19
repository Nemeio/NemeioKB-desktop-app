namespace Nemeio.Core.Applications
{
    public interface IApplicationController
    {
        bool Started { get; }
        void Start();
        void ShutDown();
    }
}
