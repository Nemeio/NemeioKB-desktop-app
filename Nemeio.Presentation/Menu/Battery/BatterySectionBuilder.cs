using System;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Managers;
using Nemeio.Core.Services.Batteries;
using Nemeio.Presentation.Menu.Tools;

namespace Nemeio.Presentation.Menu.Battery
{
    public sealed class BatterySectionBuilder : SectionBuilder<BatterySection, BatteryInformation>
    {
        private const int BatteryTwentyPercent = 20;
        private const int BatteryFortyPercent = 40;
        private const int BatterySixtyPercent = 60;
        private const int BatteryEightyPercent = 80;

        public BatterySectionBuilder(ILanguageManager languageManager) 
            : base(languageManager) { }

        public override BatterySection Build(BatteryInformation informations)
        {
            if (informations == null)
            {
                return new BatterySection(
                    visible: false,
                    BatteryImageType.Level20,
                    string.Empty
                );
            }

            return new BatterySection(
                visible: true,
                BuildImageType(informations),
                BuildText(informations)
            );
        }

        private BatteryImageType BuildImageType(BatteryInformation informations)
        {
            var batteryImageType = BatteryImageType.Level20;
            var batteryLevel = informations.Level;

            if (batteryLevel != null)
            {
                if (batteryLevel > BatteryEightyPercent)
                {
                    batteryImageType = BatteryImageType.Level100;
                }
                else if (batteryLevel > BatterySixtyPercent)
                {
                    batteryImageType = BatteryImageType.Level80;
                }
                else if (batteryLevel > BatteryFortyPercent)
                {
                    batteryImageType = BatteryImageType.Level60;
                }
                else if (batteryLevel > BatteryTwentyPercent)
                {
                    batteryImageType = BatteryImageType.Level40;
                }
            }

            return batteryImageType;
        }

        private string BuildText(BatteryInformation informations)
        {
            var batteryLevelValue = informations.Level.ToString() ?? string.Empty;
            var currentLevelMessage = LanguageManager.GetLocalizedValue(StringId.BatteryMessageCurrentLevel);

            return $"{currentLevelMessage} {batteryLevelValue}%";
        }
    }
}
