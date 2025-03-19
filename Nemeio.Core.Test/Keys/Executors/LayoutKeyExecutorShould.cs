using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Errors;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Keys.Executors;
using Nemeio.Core.Layouts.Active;
using Nemeio.Core.Services.Layouts;
using NUnit.Framework;

namespace Nemeio.Core.Test.Keys.Executors
{
    public class LayoutKeyExecutorShould
    {
        [Test]
        public void LayoutKeyExecutor_Constructor_Ok()
        {
            var loggerFactory = new LoggerFactory();
            var mockErrorManager = Mock.Of<IErrorManager>();
            var mockActiveLayoutChangeHandler = Mock.Of<IActiveLayoutChangeHandler>();
            var mockNemeio = Mock.Of<INemeio>();

            Assert.Throws<ArgumentNullException>(() => new LayoutKeyExecutor(null, mockActiveLayoutChangeHandler, mockErrorManager, mockNemeio, string.Empty));
            Assert.Throws<ArgumentNullException>(() => new LayoutKeyExecutor(loggerFactory, null, mockErrorManager, mockNemeio, string.Empty));
            Assert.Throws<ArgumentNullException>(() => new LayoutKeyExecutor(loggerFactory, mockActiveLayoutChangeHandler, null, mockNemeio, string.Empty));
            Assert.Throws<ArgumentNullException>(() => new LayoutKeyExecutor(null, null, null, mockNemeio, string.Empty));

            Assert.DoesNotThrow(() => new LayoutKeyExecutor(loggerFactory, mockActiveLayoutChangeHandler, mockErrorManager, mockNemeio, string.Empty));
        }

        [Test]
        public async Task LayoutKeyExecutor_Execute_WhenDataIsValid_ChangeLayout()
        {
            const string layoutId = "AD36EF98-B3AC-4D36-BBD8-56ABD736D31A";

            var ChangeRequestKeyPressChangeAsync = false;

            var loggerFactory = new LoggerFactory();
            var mockErrorManager = Mock.Of<IErrorManager>();
            var mockActiveLayoutChangeHandler = Mock.Of<IActiveLayoutChangeHandler>();
            var mockNemeio = Mock.Of<INemeio>();

            Mock.Get(mockActiveLayoutChangeHandler)
                .Setup(mock => mock.RequestKeyPressChangeAsync(mockNemeio, It.IsAny<LayoutId>()))
                .Callback(() => ChangeRequestKeyPressChangeAsync = true);

            var executor = new LayoutKeyExecutor(loggerFactory, mockActiveLayoutChangeHandler, mockErrorManager, mockNemeio, layoutId);

            await executor.ExecuteAsync();

            ChangeRequestKeyPressChangeAsync.Should().BeTrue();
        }

        [TestCase("")]
        [TestCase("this_is_not_a_valid_layout_id")]
        public async Task LayoutKeyExecutor_Execute_WhenDataIsInvalid_LogError(string layoutId)
        {
            var ChangeRequestKeyPressChangeAsync = false;

            var loggerFactory = new LoggerFactory();
            var mockErrorManager = Mock.Of<IErrorManager>();
            var mockActiveLayoutChangeHandler = Mock.Of<IActiveLayoutChangeHandler>();
            var mockNemeio = Mock.Of<INemeio>();

            Mock.Get(mockActiveLayoutChangeHandler)
                .Setup(mock => mock.RequestKeyPressChangeAsync(mockNemeio, It.IsAny<LayoutId>()))
                .Callback(() => ChangeRequestKeyPressChangeAsync = true);

            var executor = new LayoutKeyExecutor(loggerFactory, mockActiveLayoutChangeHandler, mockErrorManager, mockNemeio, string.Empty);

            await executor.ExecuteAsync();

            ChangeRequestKeyPressChangeAsync.Should().BeFalse();
        }
    }
}
