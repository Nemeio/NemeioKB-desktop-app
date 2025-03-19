using System;
using FluentAssertions;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Enums;
using NUnit.Framework;

namespace Nemeio.Core.Test
{
    [TestFixture]
    public class KeySubActionShould
    {
        [Test]
        public void KeySubAction_01_01_Constructor_WorksOk()
        {
            Assert.DoesNotThrow(() => new KeySubAction("a", KeyActionType.Unicode));
            Assert.Throws<ArgumentNullException>(() => new KeySubAction(null, KeyActionType.Unicode));
        }

        [Test]
        public void KeySubAction_02_01_IsShift_WorksOk()
        {
            var keySubAction = new KeySubAction(KeyboardLiterals.Shift, KeyActionType.Unicode);
            
            keySubAction.IsShift().Should().BeFalse();
            keySubAction.IsAnyModifier().Should().BeFalse();
            keySubAction.IsAlt().Should().BeFalse();
            keySubAction.IsCtrl().Should().BeFalse();
            keySubAction.IsAltGr().Should().BeFalse();
            keySubAction.IsFunction().Should().BeFalse();

            keySubAction = new KeySubAction(KeyboardLiterals.Shift, KeyActionType.Special);

            keySubAction.IsShift().Should().BeTrue();
            keySubAction.IsAnyModifier().Should().BeTrue();
            keySubAction.IsAlt().Should().BeFalse();
            keySubAction.IsCtrl().Should().BeFalse();
            keySubAction.IsAltGr().Should().BeFalse();
            keySubAction.IsFunction().Should().BeFalse();
        }

        [Test]
        public void KeySubAction_02_02_IsAlt_WorksOk()
        {
            var keySubAction = new KeySubAction(KeyboardLiterals.Alt, KeyActionType.Unicode);
            keySubAction.IsShift().Should().BeFalse();
            keySubAction.IsAnyModifier().Should().BeFalse();
            keySubAction.IsAlt().Should().BeFalse();
            keySubAction.IsCtrl().Should().BeFalse();
            keySubAction.IsAltGr().Should().BeFalse();
            keySubAction.IsFunction().Should().BeFalse();

            keySubAction = new KeySubAction(KeyboardLiterals.Alt, KeyActionType.Special);
            keySubAction.IsShift().Should().BeFalse();
            keySubAction.IsAnyModifier().Should().BeFalse();
            keySubAction.IsAlt().Should().BeTrue();
            keySubAction.IsCtrl().Should().BeFalse();
            keySubAction.IsAltGr().Should().BeFalse();
            keySubAction.IsFunction().Should().BeFalse();
        }

        [Test]
        public void KeySubAction_02_03_IsAltGr_WorksOk()
        {
            var keySubAction = new KeySubAction(KeyboardLiterals.AltGr, KeyActionType.Unicode);
            keySubAction.IsShift().Should().BeFalse();
            keySubAction.IsAnyModifier().Should().BeFalse();
            keySubAction.IsAlt().Should().BeFalse();
            keySubAction.IsCtrl().Should().BeFalse();
            keySubAction.IsAltGr().Should().BeFalse();
            keySubAction.IsFunction().Should().BeFalse();

            keySubAction = new KeySubAction(KeyboardLiterals.AltGr, KeyActionType.Special);
            keySubAction.IsShift().Should().BeFalse();
            keySubAction.IsAnyModifier().Should().BeTrue();
            keySubAction.IsAlt().Should().BeFalse();
            keySubAction.IsCtrl().Should().BeFalse();
            keySubAction.IsAltGr().Should().BeTrue();
            keySubAction.IsFunction().Should().BeFalse();
        }

        [Test]
        public void KeySubAction_02_04_IsCtrl_WorksOk()
        {
            var keySubAction = new KeySubAction(KeyboardLiterals.Ctrl, KeyActionType.Unicode);
            keySubAction.IsShift().Should().BeFalse();
            keySubAction.IsAnyModifier().Should().BeFalse();
            keySubAction.IsAlt().Should().BeFalse();
            keySubAction.IsCtrl().Should().BeFalse();
            keySubAction.IsAltGr().Should().BeFalse();
            keySubAction.IsFunction().Should().BeFalse();

            keySubAction = new KeySubAction(KeyboardLiterals.Ctrl, KeyActionType.Special);
            keySubAction.IsShift().Should().BeFalse();
            keySubAction.IsAnyModifier().Should().BeFalse();
            keySubAction.IsAlt().Should().BeFalse();
            keySubAction.IsCtrl().Should().BeTrue();
            keySubAction.IsAltGr().Should().BeFalse();
            keySubAction.IsFunction().Should().BeFalse();
        }

        [Test]
        public void KeySubAction_02_05_IsFunction_WorksOk()
        {
            var keySubAction = new KeySubAction(KeyboardLiterals.Fn, KeyActionType.Unicode);
            keySubAction.IsShift().Should().BeFalse();
            keySubAction.IsAnyModifier().Should().BeFalse();
            keySubAction.IsAlt().Should().BeFalse();
            keySubAction.IsCtrl().Should().BeFalse();
            keySubAction.IsAltGr().Should().BeFalse();
            keySubAction.IsFunction().Should().BeFalse();

            keySubAction = new KeySubAction(KeyboardLiterals.Fn, KeyActionType.Special);
            keySubAction.IsShift().Should().BeFalse();
            keySubAction.IsAnyModifier().Should().BeFalse();
            keySubAction.IsAlt().Should().BeFalse();
            keySubAction.IsCtrl().Should().BeFalse();
            keySubAction.IsAltGr().Should().BeFalse();
            keySubAction.IsFunction().Should().BeTrue();
        }
    }
}
