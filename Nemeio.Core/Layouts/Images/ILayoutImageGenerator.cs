using System.Collections.Generic;
using System.IO;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Images;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Layouts.Images
{
    public interface ILayoutImageGenerator
    {
        Stream LoadEmbeddedResource(string name);
        IEnumerable<Key> CreateLayoutKeys(IScreen screen, OsLayoutId layoutId);
        byte[] RenderLayoutImage(ImageRequest request);
    }
}
