using System;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Managers;

namespace Nemeio.Core.Services.Batteries
{
    public class BatteryTooltipComposer
    {
        private ILanguageManager _languageManager;

        public BatteryTooltipComposer(ILanguageManager languageManager)
        {
            _languageManager = languageManager ?? throw new ArgumentNullException(nameof(languageManager));
        }

        public string GetTooltipText(BatteryInformation informations)
        {
            if (informations == null)
            {
                throw new ArgumentNullException(nameof(informations));
            }

            var tooltip = string.Empty;

            tooltip += $"{_languageManager.GetLocalizedValue(StringId.BatteryMessageCurrentLevel)} {informations.Level}%";

            var timeToFull = informations.TimeToFull;
            if (timeToFull.Interval != TimeSpan.Zero)
            {
                if (!timeToFull.StandBy)
                {
                    tooltip += Environment.NewLine;
                    tooltip += $"{_languageManager.GetLocalizedValue(StringId.BatteryTimeToFull)}: {GetTimeSpanReadable(timeToFull.Interval)}";
                }
            }

            var timeToEmpty = informations.TimeToEmpty;
            if (timeToEmpty.Interval != TimeSpan.Zero)
            {
                if (!timeToEmpty.StandBy)
                {
                    tooltip += Environment.NewLine;
                    tooltip += $"{_languageManager.GetLocalizedValue(StringId.BatteryTimeToEmpty)}: {GetTimeSpanReadable(timeToEmpty.Interval)}";
                }
            }

            return tooltip;
        }

        private string GetTimeSpanReadable(TimeSpan timeSpan)
        {
            var dayValue = _languageManager.GetLocalizedValue(StringId.CommonDayGlyph);
            var hourValue = _languageManager.GetLocalizedValue(StringId.CommonHourGlyph);
            var minuteValue = _languageManager.GetLocalizedValue(StringId.CommonMinuteGlyph);
            var secondValue = _languageManager.GetLocalizedValue(StringId.CommonSecondGlyph);

            var formatted = string.Empty;

            if (timeSpan.Days > 0 || timeSpan.Hours > 0 || timeSpan.Minutes > 0)
            {
                formatted = string.Format("{0}{1}{2}",
                    timeSpan.Duration().Days > 0 ? string.Format("{0:0}" + dayValue, timeSpan.Days) : string.Empty,
                    timeSpan.Duration().Hours > 0 ? string.Format("{0:0}" + hourValue, timeSpan.Hours) : string.Empty,
                    timeSpan.Duration().Minutes > 0 ? string.Format("{0:0}" + minuteValue, timeSpan.Minutes) : string.Empty);
            }
            else
            {
                formatted = string.Format(
                    "{0}",
                    timeSpan.Duration().Seconds > 0 ? string.Format("{0:0}" + secondValue, timeSpan.Seconds) : string.Empty
                );
            }

            if (formatted.EndsWith(", "))
            {
                formatted = formatted.Substring(0, formatted.Length - 2);
            }

            if (string.IsNullOrEmpty(formatted))
            {
                formatted = $"0 {secondValue}";
            }

            return formatted;
        }
    }
}
