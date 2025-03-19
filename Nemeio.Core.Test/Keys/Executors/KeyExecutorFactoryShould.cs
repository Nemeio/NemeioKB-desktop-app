using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Enums;
using Nemeio.Core.Errors;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Keys.Executors;
using Nemeio.Core.Layouts.Active;
using Nemeio.Core.Managers;
using Nemeio.Core.Services;
using Nemeio.Core.Systems;
using NUnit.Framework;

namespace Nemeio.Core.Test.Keys.Executors
{
    public class KeyExecutorFactoryShould
    {
        [Test]
        public void KeyExecutorFactory_Constructor_Ok()
        {
            var loggerFactory = new LoggerFactory();
            var mockErrorManager = Mock.Of<IErrorManager>();
            var mockBrowserFile = Mock.Of<IBrowserFile>();
            var mockDialogService = Mock.Of<IDialogService>();
            var mockLanguageManager = Mock.Of<ILanguageManager>();
            var mockSystem = Mock.Of<ISystem>();
            var mockActiveLayoutChangeHandler = Mock.Of<IActiveLayoutChangeHandler>();

            Assert.Throws<ArgumentNullException>(() => new KeyExecutorFactory(null, mockSystem, mockBrowserFile, mockDialogService, mockErrorManager, mockActiveLayoutChangeHandler));
            Assert.Throws<ArgumentNullException>(() => new KeyExecutorFactory(loggerFactory, null, mockBrowserFile, mockDialogService, mockErrorManager, mockActiveLayoutChangeHandler));
            Assert.Throws<ArgumentNullException>(() => new KeyExecutorFactory(loggerFactory, mockSystem, null, mockDialogService, mockErrorManager, mockActiveLayoutChangeHandler));
            Assert.Throws<ArgumentNullException>(() => new KeyExecutorFactory(loggerFactory, mockSystem, mockBrowserFile, null, mockErrorManager, mockActiveLayoutChangeHandler));
            Assert.Throws<ArgumentNullException>(() => new KeyExecutorFactory(loggerFactory, mockSystem, mockBrowserFile, mockDialogService, null, mockActiveLayoutChangeHandler));
            Assert.Throws<ArgumentNullException>(() => new KeyExecutorFactory(loggerFactory, mockSystem, mockBrowserFile, mockDialogService, mockErrorManager, null));
            Assert.Throws<ArgumentNullException>(() => new KeyExecutorFactory(null, null, null, null, null, null));

            Assert.DoesNotThrow(() => new KeyExecutorFactory(loggerFactory, mockSystem, mockBrowserFile, mockDialogService, mockErrorManager, mockActiveLayoutChangeHandler));
        }

        [Test]
        public void KeyExecutorFactory_Create_Application_Ok()
        {
            var applicationPath = @"C:\Windows\System32\notepad.exe";

            var result = CreateExecutor(applicationPath, KeyActionType.Application);

            result.Should().NotBeNull();
            result.Count().Should().Be(1);
            result.First().Data.Should().Be(applicationPath);
            result.First().GetType().Should().Be(typeof(ApplicationKeyExecutor));
        }

        [Test]
        public void KeyExecutorFactory_Create_Url_Ok()
        {
            var url = "https://www.google.fr";

            var result = CreateExecutor(url, KeyActionType.Url);

            result.Should().NotBeNull();
            result.Count().Should().Be(1);
            result.First().Data.Should().Be(url);
            result.First().GetType().Should().Be(typeof(UrlKeyExecutor));
        }

        [Test]
        public void KeyExecutorFactory_Create_Unicode_Ok()
        {
            var unicode = "A";

            var result = CreateExecutor(unicode, KeyActionType.Unicode);

            result.Should().NotBeNull();
            result.Count().Should().Be(1);
            result.First().Data.Should().Be(string.Empty);
            result.First().GetType().Should().Be(typeof(UnicodeKeyExecutor));
        }

        [Test]
        public void KeyExecutorFactory_Create_Special_Ok()
        {
            var special = "Shift";

            var result = CreateExecutor(special, KeyActionType.Special);

            result.Should().NotBeNull();
            result.Count().Should().Be(1);
            result.First().Data.Should().Be(string.Empty);
            result.First().GetType().Should().Be(typeof(UnicodeKeyExecutor));
        }

        [Test]
        public void KeyExecutorFactory_Create_Layout_Ok()
        {
            var layoutId = "AF3AF64E-C4F6-4EB9-9F89-25E7A4891D35";

            var result = CreateExecutor(layoutId, KeyActionType.Layout);

            result.Should().NotBeNull();
            result.Count().Should().Be(1);
            result.First().Data.Should().Be(layoutId);
            result.First().GetType().Should().Be(typeof(LayoutKeyExecutor));
        }

        private IEnumerable<KeyExecutor> CreateExecutor(string data, KeyActionType actionType)
        {
            var factory = CreateFactory();
            var layoutSubAction = new KeySubAction(data, actionType);
            var nemeio = Mock.Of<INemeio>();

            return factory.Create(nemeio, new List<KeySubAction>() { layoutSubAction });
        }

        private KeyExecutorFactory CreateFactory()
        {
            var loggerFactory = new LoggerFactory();
            var mockErrorManager = Mock.Of<IErrorManager>();
            var mockBrowserFile = Mock.Of<IBrowserFile>();
            var mockDialogService = Mock.Of<IDialogService>();
            var mockSystem = Mock.Of<ISystem>();
            var mockActiveLayoutChangeHandler = Mock.Of<IActiveLayoutChangeHandler>();

            return new KeyExecutorFactory(loggerFactory, mockSystem, mockBrowserFile, mockDialogService, mockErrorManager, mockActiveLayoutChangeHandler);
        }
    }
}
