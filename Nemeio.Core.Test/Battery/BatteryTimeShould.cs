using FluentAssertions;
using Nemeio.Core.Services.Batteries;
using NUnit.Framework;

namespace Nemeio.Core.Test.Battery
{
    public class BatteryTimeShould
    {
        [Test]
        public void BatteryTime_Constructor_TimeValueIsFull_Ok()
        {
            var timeNotFull = new BatteryTime(0x32FF32FF);
            timeNotFull.StandBy.Should().BeFalse();

            var timeFull = new BatteryTime(BatteryTime.StandByValue);
            timeFull.StandBy.Should().BeTrue();
        }
    }
}
