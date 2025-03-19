namespace Nemeio.Core.PackageUpdater.Updatable.States
{
    public class UpdateDownloadingState : UpdateState
    {
        public int FileCount { get; private set; } = 0;
        public int FileIndex { get; private set; } = 0;
        public double Percent { get; private set; } = 0;

        public void UpdateDownloadProgress(int fileCount, int fileIndex, double percent)
        {
            FileCount = fileCount;
            FileIndex = fileIndex;
            Percent = percent;
        }
    }
}
