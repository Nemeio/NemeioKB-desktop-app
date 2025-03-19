using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Nemeio.Core;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Enums;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Services;
using Nemeio.Core.Services.Layouts;
using NUnit.Framework;

namespace Nemeio.Presentation.Test
{
    //  FIXME
    /*[TestFixture]
    public class LayoutValidityCheckerShould
    {
        private ILayoutValidityChecker _layoutValidityChecker;
        private List<Key> _fakeLayoutKeys;
        private byte[] _realKeyboardWallpaper;

        [SetUp]
        public void SetUp()
        {
            var loggerFactory = new LoggerFactory();

            _fakeLayoutKeys = new List<Key>();
            _layoutValidityChecker = new LayoutValidityChecker(loggerFactory, new TestableKeyboardMapFactory());
            _realKeyboardWallpaper = ByteArrayFromResources("Resources.azerty.wallpaper");

            for (var i = 0; i < NemeioConstants.KeyboardNumberOfKeys; i++)
            {
                _fakeLayoutKeys.Add(
                    GetFakeKey(i)
                );
            }
        }

        [Test]
        public void LayoutValidityChecker_01_01_Check_WithValidLayout_ReturnTrue()
        {
            var layout = new Layout(
                new LayoutInfo(
                    new OsLayoutId("5869742"),
                    false,
                    true,
                    new List<string>() { "C:/Program Files/Fork/fork.exe" },
                    true,
                    true,
                    true
                ),
                new LayoutImageInfo(true, FontProvider.GetDefaultFont()),
                _realKeyboardWallpaper,
                123,
                0,
                "Layout title",
                DateTime.Now,
                DateTime.Now,
                _fakeLayoutKeys,
                LayoutId.NewLayoutId,
                null,
                false,
                true
            );

            _layoutValidityChecker.Check(layout).Should().BeTrue();
        }

        [Test]
        public void LayoutValidityChecker_02_01_Check_WithNullOrWhitespace_OsLayoutId_ReturnFalse()
        {
            var layout = new Layout(
                new LayoutInfo(
                    new OsLayoutId(string.Empty),
                    false,
                    true,
                    new List<string>() { "C:/Program Files/Fork/fork.exe" },
                    true,
                    true,
                    true
                ),
                new LayoutImageInfo(true, FontProvider.GetDefaultFont()),
                _realKeyboardWallpaper,
                123,
                0,
                "My test layout",
                DateTime.Now,
                DateTime.Now,
                _fakeLayoutKeys,
                LayoutId.NewLayoutId,
                null,
                false,
                true
            );

            _layoutValidityChecker.Check(layout).Should().BeFalse();
        }

        [Test]
        public void LayoutValidityChecker_02_02_Check_WithNullOrWhitespace_Title_ReturnFalse()
        {
            var layout = new Layout(
                new LayoutInfo(
                    new OsLayoutId("5869742"),
                    false,
                    true,
                    new List<string>() { "C:/Program Files/Fork/fork.exe" },
                    true,
                    true,
                    true
                ),
                new LayoutImageInfo(true, FontProvider.GetDefaultFont()),
                _realKeyboardWallpaper,
                123,
                0,
                null,
                DateTime.Now,
                DateTime.Now,
                _fakeLayoutKeys,
                LayoutId.NewLayoutId,
                null,
                false,
                true
            );

            _layoutValidityChecker.Check(layout).Should().BeFalse();

            layout = new Layout(
                new LayoutInfo(
                    new OsLayoutId("5869742"),
                    false,
                    true,
                    new List<string>() { "C:/Program Files/Fork/fork.exe" },
                    true,
                    true,
                    true
                ),
                new LayoutImageInfo(true, FontProvider.GetDefaultFont()),
                _realKeyboardWallpaper,
                123,
                0,
                " ",
                DateTime.Now,
                DateTime.Now,
                _fakeLayoutKeys,
                LayoutId.NewLayoutId,
                null,
                false,
                true
            );

            _layoutValidityChecker.Check(layout).Should().BeFalse();

            layout = new Layout(
                new LayoutInfo(
                    new OsLayoutId("5869742"),
                    false,
                    true,
                    new List<string>() { "C:/Program Files/Fork/fork.exe" },
                    true,
                    true,
                    true
                ),
                new LayoutImageInfo(true, FontProvider.GetDefaultFont()),
                _realKeyboardWallpaper,
                123,
                0,
                string.Empty,
                DateTime.Now,
                DateTime.Now,
                _fakeLayoutKeys,
                LayoutId.NewLayoutId,
                null,
                false,
                true
            );

            _layoutValidityChecker.Check(layout).Should().BeFalse();
        }

        [Test]
        public void LayoutValidityChecker_02_03_Check_WithBlank_Image_ReturnFalse()
        {
            var layout = new Layout(
                new LayoutInfo(
                    new OsLayoutId("5869742"),
                    false,
                    true,
                    new List<string>() { "C:/Program Files/Fork/fork.exe" },
                    true,
                    true,
                    true
                ),
                new LayoutImageInfo(true, FontProvider.GetDefaultFont()),
                new byte[0],
                123,
                0,
                "My layout title",
                DateTime.Now,
                DateTime.Now,
                _fakeLayoutKeys,
                LayoutId.NewLayoutId,
                null,
                false,
                true
            );

            _layoutValidityChecker.Check(layout).Should().BeFalse();
        }

        [Test]
        public void LayoutValidityChecker_02_04_Check_WithoutAny_Keys_ReturnFalse()
        {
            var layout = new Layout(
                new LayoutInfo(
                    new OsLayoutId("5869742"),
                    false,
                    true,
                    new List<string>() { "C:/Program Files/Fork/fork.exe" },
                    true,
                    true,
                    true
                ),
                new LayoutImageInfo(true, FontProvider.GetDefaultFont()),
                _realKeyboardWallpaper,
                123,
                0,
                null,
                DateTime.Now,
                DateTime.Now,
                null,
                LayoutId.NewLayoutId,
                null,
                false,
                true
            );

            _layoutValidityChecker.Check(layout).Should().BeFalse();
        }

        [Test]
        public void LayoutValidityChecker_02_05_Check_BadCountOf_Keys_ReturnFalse()
        {
            var lowNumberOfKeys = new List<Key>()
            {
                GetFakeKey(0),
                GetFakeKey(1),
                GetFakeKey(2)
            };

            var layout = new Layout(
                new LayoutInfo(
                    new OsLayoutId("5869742"),
                    false,
                    true,
                    new List<string>() { "C:/Program Files/Fork/fork.exe" },
                    true,
                    true,
                    true
                ),
                new LayoutImageInfo(true, FontProvider.GetDefaultFont()),
                _realKeyboardWallpaper,
                123,
                0,
                null,
                DateTime.Now,
                DateTime.Now,
                lowNumberOfKeys,
                LayoutId.NewLayoutId,
                null,
                false,
                true
            );

            _layoutValidityChecker.Check(layout).Should().BeFalse();
        }

        private Key GetFakeKey(int index)
        {
            return new Key()
            {
                Index = index,
                Disposition = KeyDisposition.Single,
                Actions = new List<KeyAction>()
            };
        }

        private byte[] ByteArrayFromResources(string name)
        {
            var asm = Assembly.GetExecutingAssembly();
            var resource = string.Format("Nemeio.Presentation.Test.{0}", name);

            using (var stream = asm.GetManifestResourceStream(resource))
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);

                return ms.ToArray();
            }
        }
    }*/
}
