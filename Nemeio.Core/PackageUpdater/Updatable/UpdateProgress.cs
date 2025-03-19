using Nemeio.Core.Keyboard.Updates.Progress;

namespace Nemeio.Core.PackageUpdater.Updatable
{
    public class UpdateProgress
    {
        public UpdateComponent Name { get; private set; }
        public int Percent { get; private set; }

        public UpdateProgress(UpdateComponent name, int percent)
        {
            Name = name;
            Percent = percent;
        }
    }
}
