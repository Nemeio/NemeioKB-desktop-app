using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Layouts;
using Nemeio.Core.Layouts.Active;
using Nemeio.Core.Layouts.Active.Historic;
using Nemeio.Core.Layouts.Active.Requests;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;
using NUnit.Framework;

namespace Nemeio.Core.Test.Layouts.Active.Requests
{
    [TestFixture]
    internal class HidSystemLayoutChangeRequestShould
    {
        [Test]
        public async Task HidSystemLayoutChangeRequest_SynchronizeAsync_WhenProxyIsNull_ReturnNull()
        {
            var loggerFactory = new LoggerFactory();
            var system = Mock.Of<ISystem>();
            var nemeio = Mock.Of<INemeio>();
            var layoutLibrary = Mock.Of<ILayoutLibrary>();
            var layoutHistoric = Mock.Of<IActiveLayoutHistoric>();

            var hidSystemLayoutChangeRequest = new HidSystemLayoutChangeRequest(loggerFactory, system, layoutLibrary, nemeio);

            var result = await hidSystemLayoutChangeRequest.ApplyAsync(null, layoutHistoric, null);

            result.Should().BeNull();
        }

        [Test]
        public async Task HidSystemLayoutChangeRequest_SynchronizeAsync_WhenLayoutNotFound_ReturnNull()
        {
            var loggerFactory = new LoggerFactory();
            var systemSelectedLayout = new OsLayoutId("frenchLayout");
            var system = Mock.Of<ISystem>();
            Mock.Get(system)
                .Setup(x => x.SelectedLayout)
                .Returns(systemSelectedLayout);

            var layoutLibrary = Mock.Of<ILayoutLibrary>();
            Mock.Get(layoutLibrary)
                .Setup(x => x.Layouts)
                .Returns(new List<ILayout>());

            var layoutHistoric = Mock.Of<IActiveLayoutHistoric>();

            var nemeio = Mock.Of<INemeio>();
            var hidSystemLayoutChangeRequest = new HidSystemLayoutChangeRequest(loggerFactory, system, layoutLibrary, nemeio);

            var proxy = Mock.Of<ILayoutHolderNemeioProxy>();

            var result = await hidSystemLayoutChangeRequest.ApplyAsync(proxy, layoutHistoric, null);

            result.Should().BeNull();
        }

        [Test]
        public async Task HidSystemLayoutChangeRequest_SynchronizeAsync_Ok()
        {
            var loggerFactory=new LoggerFactory();  
            var systemSelectedLayout = new OsLayoutId("frenchLayout");
            var system = Mock.Of<ISystem>();
            Mock.Get(system)
                .Setup(x => x.SelectedLayout)
                .Returns(systemSelectedLayout);

            var frenchLayout = Mock.Of<ILayout>();
            Mock.Get(frenchLayout)
                .Setup(x => x.LayoutInfo)
                .Returns(new LayoutInfo(systemSelectedLayout, false, true));

            var layoutLibrary = Mock.Of<ILayoutLibrary>();
            Mock.Get(layoutLibrary)
                .Setup(x => x.Layouts)
                .Returns(new List<ILayout>() { frenchLayout });

            var layoutHistoric = Mock.Of<IActiveLayoutHistoric>();

            var nemeio = Mock.Of<INemeio>();
            var hidSystemLayoutChangeRequest = new HidSystemLayoutChangeRequest(loggerFactory, system, layoutLibrary, nemeio);

            var proxy = Mock.Of<ILayoutHolderNemeioProxy>();

            var result = await hidSystemLayoutChangeRequest.ApplyAsync(proxy, layoutHistoric, null);

            result.Should().NotBeNull();
            result.Should().Be(frenchLayout);
        }
    }
}
