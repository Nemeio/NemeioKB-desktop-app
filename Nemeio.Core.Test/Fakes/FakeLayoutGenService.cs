using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Moq;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Device;
using Nemeio.Core.Images;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Test.Fakes
{
    public class FakeLayoutGenService : ILayoutImageGenerator
    {
        public bool RenderLayoutFromJsonCalled = false;
        public IList<int> ModifierKeys = new List<int>();

        public Stream LoadEmbeddedResource(string name)
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();

            return currentAssembly.GetManifestResourceStream("Nemeio.Core.Test.Resources." + name);
        }

        public IDeviceKeyMap GetDeviceKeyMap()
        {
            var mockDeviceKeyMap = Mock.Of<IDeviceKeyMap>();
            var mockKeys = new List<uint>();

            for (uint i = 0; i < 81; i++)
            {
                mockKeys.Add(i);
            }

            Mock.Get(mockDeviceKeyMap)
                .Setup(x => x.Buttons)
                .Returns(mockKeys);

            Mock.Get(mockDeviceKeyMap)
                .Setup(x => x.IsModifierKey(It.IsAny<int>()))
                .Returns<int>((keyIndex) => ModifierKeys.Contains(keyIndex));

            return mockDeviceKeyMap;
        }

        public IEnumerable<Key> CreateLayoutKeys(IScreen screen, OsLayoutId layoutId)
        {
            return new List<Key>();
        }

        public byte[] RenderLayoutImage(ImageRequest request)
        {
            RenderLayoutFromJsonCalled = true;

            return new byte[0];
        }
    }
}
