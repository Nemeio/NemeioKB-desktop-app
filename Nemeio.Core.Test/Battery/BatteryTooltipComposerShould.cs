using System;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Managers;
using Nemeio.Core.Services.Batteries;
using Nemeio.Core.Test.Fakes;
using NUnit.Framework;

namespace Nemeio.Core.Test.Battery
{
    public class BatteryTooltipComposerShould
    {
        private ILanguageManager _languageManager;

        [SetUp]
        public void SetUp()
        {
            var loggerFactory = new LoggerFactory();
            _languageManager = new FakeLanguageManager(loggerFactory);
        }

        [Test]
        public void BatteryTooltipComposer_Constructor_WhenLanguageManagerIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new BatteryTooltipComposer(null));
        }

        [Test]
        public void BatteryTooltipComposer_GetTooltipText_WhenParameterIsNull_ThrowsArgumentNullException()
        {
            var batteryTooltipComposer = new BatteryTooltipComposer(_languageManager);

            Assert.Throws<ArgumentNullException>(() => batteryTooltipComposer.GetTooltipText(null));
        }

        [Test]
        public void BatteryTooltipComposer_GetTooltipText_WhenEveryTimeIsEqualToZero_Ok()
        {
            const ushort currentBatteryLevel = 85;

            var batteryInformation = new BatteryInformation(
                new BatteryLevel(currentBatteryLevel),
                1500,
                new BatteryTime(0),
                new BatteryTime(0),
                1500,
                3
            );

            var batteryTooltipComposer = new BatteryTooltipComposer(_languageManager);
            var tooltipText = batteryTooltipComposer.GetTooltipText(batteryInformation);
            var nbOfLines = tooltipText.Split(Environment.NewLine).Count();

            tooltipText.Should().Contain($"{currentBatteryLevel}%");
            nbOfLines.Should().Be(1);
        }

        [Test]
        public void BatteryTooltipComposer_GetTooltipText_WhenTimeFullIsNotStandBy_AndOnlySeconds_Ok()
        {
            uint timeToFullInSeconds = 20;
            var tooltipText = GetTooltipForSeconds(timeToFullInSeconds, 0);
            var nbOfLines = tooltipText.Split(Environment.NewLine).Count();

            var secondGlyph = _languageManager.GetLocalizedValue(StringId.CommonSecondGlyph);
            var timeToFullText = _languageManager.GetLocalizedValue(StringId.BatteryTimeToFull);

            tooltipText.Should().Contain($"{timeToFullText}: {timeToFullInSeconds}{secondGlyph}");
            nbOfLines.Should().Be(2);
        }

        [Test]
        public void BatteryTooltipComposer_GetTooltipText_WhenTimeFullIsNotStandBy_AndMinutes_Ok()
        {
            var tooltipText = GetTooltipForSeconds(80, 0);
            var nbOfLines = tooltipText.Split(Environment.NewLine).Count();

            var minuteGlyph = _languageManager.GetLocalizedValue(StringId.CommonMinuteGlyph);
            var timeToFullText = _languageManager.GetLocalizedValue(StringId.BatteryTimeToFull);

            tooltipText.Should().Contain($"{timeToFullText}: 1{minuteGlyph}");
            nbOfLines.Should().Be(2);
        }

        [Test]
        public void BatteryTooltipComposer_GetTooltipText_WhenTimeFullIsNotStandBy_AndHours_Ok()
        {
            var tooltipText = GetTooltipForSeconds(9000, 0);
            var nbOfLines = tooltipText.Split(Environment.NewLine).Count();
 
            var hourGlyph = _languageManager.GetLocalizedValue(StringId.CommonHourGlyph);
            var minuteGlyph = _languageManager.GetLocalizedValue(StringId.CommonMinuteGlyph);
            var timeToFullText = _languageManager.GetLocalizedValue(StringId.BatteryTimeToFull);

            tooltipText.Should().Contain($"{timeToFullText}: 2{hourGlyph}30{minuteGlyph}");
            nbOfLines.Should().Be(2);
        }

        [Test]
        public void BatteryTooltipComposer_GetTooltipText_WhenTimeFullIsNotStandBy_AndDays_Ok()
        {
            var tooltipText = GetTooltipForSeconds(216000, 0);
            var nbOfLines = tooltipText.Split(Environment.NewLine).Count();

            var dayGlyph = _languageManager.GetLocalizedValue(StringId.CommonDayGlyph);
            var hourGlyph = _languageManager.GetLocalizedValue(StringId.CommonHourGlyph);
            var timeToFullText = _languageManager.GetLocalizedValue(StringId.BatteryTimeToFull);

            tooltipText.Should().Contain($"{timeToFullText}: 2{dayGlyph}12{hourGlyph}");
            nbOfLines.Should().Be(2);
        }

        [Test]
        public void BatteryTooltipComposer_GetTooltipText_WhenTimeEmptyIsNotStandBy_AndOnlySeconds_Ok()
        {
            uint timeToEmptyInSeconds = 20;
            var tooltipText = GetTooltipForSeconds(0, timeToEmptyInSeconds);
            var nbOfLines = tooltipText.Split(Environment.NewLine).Count();

            var secondGlyph = _languageManager.GetLocalizedValue(StringId.CommonSecondGlyph);
            var timeToEmptyText = _languageManager.GetLocalizedValue(StringId.BatteryTimeToEmpty);

            tooltipText.Should().Contain($"{timeToEmptyText}: {timeToEmptyInSeconds}{secondGlyph}");
            nbOfLines.Should().Be(2);
        }

        [Test]
        public void BatteryTooltipComposer_GetTooltipText_WhenTimeEmptyIsNotStandBy_AndMinutes_Ok()
        {
            var tooltipText = GetTooltipForSeconds(0, 80);
            var nbOfLines = tooltipText.Split(Environment.NewLine).Count();

            var minuteGlyph = _languageManager.GetLocalizedValue(StringId.CommonMinuteGlyph);
            var timeToEmptyText = _languageManager.GetLocalizedValue(StringId.BatteryTimeToEmpty);

            tooltipText.Should().Contain($"{timeToEmptyText}: 1{minuteGlyph}");
            nbOfLines.Should().Be(2);
        }

        [Test]
        public void BatteryTooltipComposer_GetTooltipText_WhenTimeEmptyIsNotStandBy_AndHours_Ok()
        {
            var tooltipText = GetTooltipForSeconds(0, 9000);
            var nbOfLines = tooltipText.Split(Environment.NewLine).Count();

            var hourGlyph = _languageManager.GetLocalizedValue(StringId.CommonHourGlyph);
            var minuteGlyph = _languageManager.GetLocalizedValue(StringId.CommonMinuteGlyph);
            var timeToEmptyText = _languageManager.GetLocalizedValue(StringId.BatteryTimeToEmpty);

            tooltipText.Should().Contain($"{timeToEmptyText}: 2{hourGlyph}30{minuteGlyph}");
            nbOfLines.Should().Be(2);
        }

        [Test]
        public void BatteryTooltipComposer_GetTooltipText_WhenTimeEmptyIsNotStandBy_AndDays_Ok()
        {
            var tooltipText = GetTooltipForSeconds(0, 216000);
            var nbOfLines = tooltipText.Split(Environment.NewLine).Count();

            var dayGlyph = _languageManager.GetLocalizedValue(StringId.CommonDayGlyph);
            var hourGlyph = _languageManager.GetLocalizedValue(StringId.CommonHourGlyph);
            var timeToEmptyText = _languageManager.GetLocalizedValue(StringId.BatteryTimeToEmpty);

            tooltipText.Should().Contain($"{timeToEmptyText}: 2{dayGlyph}12{hourGlyph}");
            nbOfLines.Should().Be(2);
        }

        [Test]
        public void BatteryTooltipComposer_GetTooltipText_WhenTimesIsNotStandBy_Ok()
        {
            const ushort currentBatteryLevel = 85;
            const uint timeToFullInSeconds = 9000;
            const uint timeToEmptyInSeconds = 216000;

            var batteryInformation = new BatteryInformation(
                new BatteryLevel(currentBatteryLevel),
                1500,
                new BatteryTime(timeToFullInSeconds),
                new BatteryTime(timeToEmptyInSeconds),
                1500,
                3
            );

            var batteryTooltipComposer = new BatteryTooltipComposer(_languageManager);

            var tooltipText = batteryTooltipComposer.GetTooltipText(batteryInformation);
            var textLines = tooltipText.Split(Environment.NewLine);
            var nbOfLines = textLines.Count();

            var dayGlyph = _languageManager.GetLocalizedValue(StringId.CommonDayGlyph);
            var hourGlyph = _languageManager.GetLocalizedValue(StringId.CommonHourGlyph);
            var minuteGlyph = _languageManager.GetLocalizedValue(StringId.CommonMinuteGlyph);
            var timeToEmptyText = _languageManager.GetLocalizedValue(StringId.BatteryTimeToEmpty);
            var timeToFullText = _languageManager.GetLocalizedValue(StringId.BatteryTimeToFull);
            var batteryText = _languageManager.GetLocalizedValue(StringId.BatteryMessageCurrentLevel);

            textLines[0].Should().Be($"{batteryText} {currentBatteryLevel}%");
            textLines[1].Should().Be($"{timeToFullText}: 2{hourGlyph}30{minuteGlyph}");
            textLines[2].Should().Be($"{timeToEmptyText}: 2{dayGlyph}12{hourGlyph}");
            nbOfLines.Should().Be(3);
        }

        private string GetTooltipForSeconds(uint timeToFullInSeconds, uint timeToEmptyInSeconds)
        {
            var batteryInformation = new BatteryInformation(
                new BatteryLevel(85),
                1500,
                new BatteryTime(timeToFullInSeconds),
                new BatteryTime(timeToEmptyInSeconds),
                1500,
                3
            );

            var batteryTooltipComposer = new BatteryTooltipComposer(_languageManager);
            
            return batteryTooltipComposer.GetTooltipText(batteryInformation);
        }
    }
}
