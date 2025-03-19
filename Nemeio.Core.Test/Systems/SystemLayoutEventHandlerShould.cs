using Moq;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Layouts.Active;
using Nemeio.Core.Systems;
using Nemeio.Core.Systems.Layouts;
using NUnit.Framework;
using System;

namespace Nemeio.Core.Test.Systems
{
    [TestFixture]
    public class SystemLayoutEventHandlerShould
    {
        [Test]
        public void SystemLayoutEventHandler_Constructor_Ok()
        {
            var system = Mock.Of<ISystem>();
            var keyboardController = Mock.Of<IKeyboardController>();
            var activeLayoutChangeHandler = Mock.Of<IActiveLayoutChangeHandler>();

            Assert.DoesNotThrow(() => new SystemLayoutEventHandler(system, keyboardController, activeLayoutChangeHandler));
            Assert.Throws<ArgumentNullException>(() => new SystemLayoutEventHandler(null, keyboardController, activeLayoutChangeHandler));
            Assert.Throws<ArgumentNullException>(() => new SystemLayoutEventHandler(system, null, activeLayoutChangeHandler));
            Assert.Throws<ArgumentNullException>(() => new SystemLayoutEventHandler(system, keyboardController, null));

        }
    }
}
