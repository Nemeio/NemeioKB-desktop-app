using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Errors;
using Nemeio.Core.Layouts;
using Nemeio.Core.Layouts.Name;
using Nemeio.Core.Services;
using Nemeio.Core.Services.Layouts;
using NUnit.Framework;

namespace Nemeio.Core.Test.Layouts
{
    [TestFixture]
    public class LayoutLibraryShould
    {
        private ILoggerFactory _loggerFactory;
        private ILayoutDbRepository _repository;
        private IErrorManager _errorManager;
        private ILayoutValidityChecker _validtityChecker;
        private LayoutLibrary _library;
        private ILayoutNativeNameAdapter _layoutNativeNameAdapter;

        [SetUp]
        public void SetUp()
        {
            _loggerFactory = new LoggerFactory();
            _repository = Mock.Of<ILayoutDbRepository>();
            _errorManager = Mock.Of<IErrorManager>();
            _validtityChecker = Mock.Of<ILayoutValidityChecker>();
            _layoutNativeNameAdapter = Mock.Of<ILayoutNativeNameAdapter>();
            _library = new LayoutLibrary(_loggerFactory, _validtityChecker, _repository, _errorManager, _layoutNativeNameAdapter);
        }

        [Test]
        public void LayoutLibrary_AddLayoutAsync_WhenInvalid_Throws()
        {
            var repositoryCreateLayoutCalled = false;

            var layout = Mock.Of<ILayout>();

            Mock.Get(_repository)
                .Setup(x => x.CreateLayout(It.IsAny<ILayout>()))
                .Callback(() => repositoryCreateLayoutCalled = true);

            Mock.Get(_validtityChecker)
                .Setup(x => x.Check(It.IsAny<ILayout>()))
                .Returns(false);

            _library.Layouts.Count.Should().Be(0);

            Assert.ThrowsAsync<InvalidOperationException>(() => _library.AddLayoutAsync(layout));

            _library.Layouts.Count.Should().Be(0);
            repositoryCreateLayoutCalled.Should().BeFalse();
        }

        [Test]
        public async Task LayoutLibrary_AddLayoutAsync_Ok()
        {
            var repositoryCreateLayoutCalled = false;

            var layout = Mock.Of<ILayout>();

            Mock.Get(_validtityChecker)
                .Setup(x => x.Check(It.IsAny<ILayout>()))
                .Returns(true);

            Mock.Get(_repository)
                .Setup(x => x.CreateLayout(It.IsAny<ILayout>()))
                .Callback(() => repositoryCreateLayoutCalled = true);

            _library.Layouts.Count.Should().Be(0);

            await _library.AddLayoutAsync(layout);

            _library.Layouts.Count.Should().Be(1);
            repositoryCreateLayoutCalled.Should().BeTrue();
        }

        [Test]
        public async Task LayoutLibrary_RemoveLayoutAsync_Ok()
        {
            var repositoryRemoveLayoutCalled = false;

            var addedLayout = Mock.Of<ILayout>();
            Mock.Get(addedLayout)
                .Setup(x => x.Title)
                .Returns("MyFirstAddedLayout");

            Mock.Get(_validtityChecker)
                .Setup(x => x.Check(It.IsAny<ILayout>()))
                .Returns(true);

            Mock.Get(_repository)
                .Setup(x => x.DeleteLayout(It.IsAny<ILayout>()))
                .Callback(() => repositoryRemoveLayoutCalled = true);

            await _library.AddLayoutAsync(addedLayout);

            _library.Layouts.Count.Should().Be(1);

            await _library.RemoveLayoutAsync(addedLayout);

            _library.Layouts.Count.Should().Be(0);
            repositoryRemoveLayoutCalled.Should().BeTrue();
        }

        [Test]
        public async Task LayoutLibrary_UpdateLayoutAsync_Ok()
        {
            var repositoryUpdateLayoutCalled = false;

            var addedLayout = Mock.Of<ILayout>();
            Mock.Get(addedLayout)
                .Setup(x => x.Title)
                .Returns("MyFirstAddedLayout");

            Mock.Get(_validtityChecker)
                .Setup(x => x.Check(It.IsAny<ILayout>()))
                .Returns(true);

            Mock.Get(_repository)
                .Setup(x => x.UpdateLayout(It.IsAny<ILayout>()))
                .Callback(() => repositoryUpdateLayoutCalled = true);

            await _library.AddLayoutAsync(addedLayout);

            _library.Layouts.Count.Should().Be(1);

            var updatedTitle = "MyUpdatedAddedLayout";
            Mock.Get(addedLayout)
                .Setup(x => x.Title)
                .Returns(updatedTitle);

            var updatedLayout = await _library.UpdateLayoutAsync(addedLayout);

            updatedLayout.Should().NotBeNull();
            updatedLayout.Title.Should().Be(updatedTitle);
            _library.Layouts.Count.Should().Be(1);
            repositoryUpdateLayoutCalled.Should().BeTrue();
        }
    }
}
