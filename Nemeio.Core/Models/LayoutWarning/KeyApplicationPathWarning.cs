using System.IO;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Enums;

namespace Nemeio.Core.Models.LayoutWarning
{
    public class KeyApplicationPathWarning : LayoutWarning
    {
        public override LayoutWarningType Type => LayoutWarningType.KeyActionApplicationPath;
        public string ApplicationPath { get; private set; }
        public string ApplicationName { get; private set; }
        public int KeyIndex { get; private set; }
        public KeyboardModifier KeyModifier { get; private set; }

        public KeyApplicationPathWarning(Key key, KeyAction action, KeySubAction subAction)
        {
            KeyIndex = key.Index;
            KeyModifier = action.Modifier;
            ApplicationPath = subAction.Data;
            ApplicationName = Path.GetFileNameWithoutExtension(ApplicationPath);
        }
    }
}
