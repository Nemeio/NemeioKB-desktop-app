using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Core.Managers;
using Nemeio.Presentation.Menu.Tools;

namespace Nemeio.Presentation.Menu.Connection
{
    public class ConnectionSectionBuilder : SectionBuilder<ConnectionSection, Core.Keyboard.Nemeios.INemeio>
    {
        public ConnectionSectionBuilder(ILanguageManager languageManager) 
            : base(languageManager) { }

        public override ConnectionSection Build(Core.Keyboard.Nemeios.INemeio keyboard)
        {
            if (keyboard == null)
            {
                return new ConnectionSection(
                    visible: false,
                    string.Empty,
                    CommunicationType.Serial
                );
            }
            else
            {
                return new ConnectionSection(
                    visible: true,
                    BuildTitle(keyboard),
                    keyboard.CommunicationType
                );
            }
        }

        private string BuildTitle(Core.Keyboard.Nemeios.INemeio keyboard)
        {
            var connectedWord = LanguageManager.GetLocalizedValue(StringId.CommonConnected);
            var appName = LanguageManager.GetLocalizedValue(StringId.ApplicationTitleName);
            
            var keyboardName = keyboard.Name;
            
            return $"{connectedWord} {appName} {keyboardName}";
        }
    }
}
