using System.Collections.Generic;

namespace Nemeio.Tools.LayoutConverter.Providers
{
    internal interface IAugmentedImageFileProvider
    {
        string LayoutId { get; set; }

        IEnumerable<string> GetFilesFrom(string folderPath);

        bool CheckEveryNeededFileArePresent(string folderPath);
    }
}
