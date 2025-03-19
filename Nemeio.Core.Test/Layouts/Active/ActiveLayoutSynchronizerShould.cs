using System;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Layouts.Active;
using Nemeio.Core.Layouts.Active.Requests.Base;
using Nemeio.Core.Layouts.Active.Requests.Factories;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;
using NUnit.Framework;

namespace Nemeio.Core.Test.Layouts.Active
{
    [TestFixture]
    public class ActiveLayoutSynchronizerShould
    {
        private ILoggerFactory _loggerFactory;
        private ILayout _layout;
        private IChangeRequest _changeRequest;
        private IChangeRequestFactory _changeRequestFactory;
        private ISystem _system;
        private IKeyboardController _keyboardController;
        private ActiveLayoutSynchronizer _activeLayoutSynchronizer;

        [SetUp]
        public void SetUp()
        {
            _loggerFactory = new LoggerFactory();

            _activeLayoutSynchronizer = new ActiveLayoutSynchronizer(_loggerFactory);
        }

        [Test]
        public void ActiveLayoutSynchronizer_PostRequestAsync_WhenRequestIsNull_Throws()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _activeLayoutSynchronizer.PostRequestAsync(null));
        }
    }
}
