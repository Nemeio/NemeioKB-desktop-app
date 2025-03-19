using System;
using FluentAssertions;
using Moq;
using Nemeio.Core.Notifier;
using Nemeio.Core.Services.Batteries;
using NUnit.Framework;

namespace Nemeio.Core.Test
{
    //  FIXME
    /*public class BatteryNotifierShould
    {
        private bool NotificationReleased { get; set; }

        private TestableBatteryNotifier _batteryNotifier;
        private IDeviceController _mockDeviceController;

        [SetUp]
        public void SetUp()
        {
            _mockDeviceController = Mock.Of<IDeviceController>();
            
            _batteryNotifier = new TestableBatteryNotifier(_mockDeviceController);
            _batteryNotifier.NotificationReleased += BatteryNotifier_NotificationReleased;
            _batteryNotifier.Start();
            _batteryNotifier.OverrideConnectionTimerStarted(DateTime.Now.AddMinutes(-1));
        }

        [TearDown]
        public void TearDown()
        {
            NotificationReleased = false;

            _batteryNotifier?.Stop();
            _batteryNotifier.NotificationReleased -= BatteryNotifier_NotificationReleased;
            _batteryNotifier = null;

            _mockDeviceController = null;
        }

        [Test]
        public void BatteryNotifier_01_01_NotificationRelease_SendNormalNotificationOnlyOnce_WorksOk()
        {
            Mock.Get(_mockDeviceController)
                .Setup(x => x.BatteryInformation)
                .Returns(new BatteryInformation(
                    new BatteryLevel((ushort)NemeioConstants.NemeioMinimumBatteryLevel),
                    0,
                    new BatteryTime(0),
                    new BatteryTime(0),
                    0,
                    0
                ));

            _batteryNotifier.NormalNotificationSended.Should().BeFalse();
            _batteryNotifier.UpdateBatteryStatus();

            _batteryNotifier.NormalNotificationSended.Should().BeTrue();
            NotificationReleased.Should().BeTrue();

            NotificationReleased = false;
            _batteryNotifier.UpdateBatteryStatus();
            NotificationReleased.Should().BeFalse();
        }

        [Test]
        public void BatteryNotifier_01_02_NotificationRelease_SendLowNotificationOnlyOnce_WorksOk()
        {
            SetBatteryLevel((ushort)NemeioConstants.NemeioLowLevelBattery);

            _batteryNotifier.LowNotificationSended.Should().BeFalse();
            _batteryNotifier.UpdateBatteryStatus();

            _batteryNotifier.LowNotificationSended.Should().BeTrue();
            NotificationReleased.Should().BeTrue();

            NotificationReleased = false;
            _batteryNotifier.UpdateBatteryStatus();
            NotificationReleased.Should().BeFalse();
        }

        [Test]
        public void BatteryNotifier_01_03_NotificationRelease_SendVeryLowNotificationOnlyOnce_WorksOk()
        {
            SetBatteryLevel((ushort)NemeioConstants.NemeioVeryLowLevelBattery);

            _batteryNotifier.VeryLowNotificationSended.Should().BeFalse();
            _batteryNotifier.UpdateBatteryStatus();

            _batteryNotifier.VeryLowNotificationSended.Should().BeTrue();
            NotificationReleased.Should().BeTrue();

            NotificationReleased = false;
            _batteryNotifier.UpdateBatteryStatus();
            NotificationReleased.Should().BeFalse();
        }

        [Test]
        public void BatteryNotifier_01_04_NotificationRelease_LevelIncrease_WorksOk()
        {
            //  Trigger each notifications
            SetBatteryLevel((ushort)NemeioConstants.NemeioMinimumBatteryLevel);
            _batteryNotifier.UpdateBatteryStatus();
            SetBatteryLevel((ushort)NemeioConstants.NemeioLowLevelBattery);
            _batteryNotifier.UpdateBatteryStatus();
            SetBatteryLevel((ushort)NemeioConstants.NemeioVeryLowLevelBattery);
            _batteryNotifier.UpdateBatteryStatus();

            NotificationReleased.Should().BeTrue();
            NotificationReleased = false;

            _batteryNotifier.NormalNotificationSended.Should().BeTrue();
            _batteryNotifier.LowNotificationSended.Should().BeTrue();
            _batteryNotifier.VeryLowNotificationSended.Should().BeTrue();

            SetBatteryLevel((ushort)(NemeioConstants.NemeioVeryLowLevelBattery + 1));
            _batteryNotifier.UpdateBatteryStatus();

            //  Check very low notification
            _batteryNotifier.NormalNotificationSended.Should().BeTrue();
            _batteryNotifier.LowNotificationSended.Should().BeTrue();
            _batteryNotifier.VeryLowNotificationSended.Should().BeFalse();

            SetBatteryLevel((ushort)(NemeioConstants.NemeioLowLevelBattery + 1));
            _batteryNotifier.UpdateBatteryStatus();

            //  Check low notification
            _batteryNotifier.NormalNotificationSended.Should().BeTrue();
            _batteryNotifier.LowNotificationSended.Should().BeFalse();
            _batteryNotifier.VeryLowNotificationSended.Should().BeFalse();

            SetBatteryLevel((ushort)(NemeioConstants.NemeioMinimumBatteryLevel + 1));
            _batteryNotifier.UpdateBatteryStatus();

            //  Check normal notification
            _batteryNotifier.NormalNotificationSended.Should().BeFalse();
            _batteryNotifier.LowNotificationSended.Should().BeFalse();
            _batteryNotifier.VeryLowNotificationSended.Should().BeFalse();
        }

        private void BatteryNotifier_NotificationReleased(object sender, EventArguments.NotificationReleaseEventArgs e) => NotificationReleased = true;

        private void SetBatteryLevel(ushort batterylevel)
        {
            Mock.Get(_mockDeviceController)
                .Setup(x => x.BatteryInformation)
                .Returns(new BatteryInformation(
                    new BatteryLevel(batterylevel),
                    0,
                    new BatteryTime(0),
                    new BatteryTime(0),
                    0,
                    0
                ));
        }
    }

    internal class TestableBatteryNotifier : BatteryNotifier
    {
        public TestableBatteryNotifier(IDeviceController deviceController)
            : base(deviceController) { }

        public void OverrideConnectionTimerStarted(DateTime dateTime) => _connectionTimerStarted = dateTime;
    }*/
}
