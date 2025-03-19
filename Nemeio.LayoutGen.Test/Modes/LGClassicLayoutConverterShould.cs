using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Enums;
using Nemeio.Core.Images;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Services;
using Nemeio.LayoutGen.Converters;
using Nemeio.LayoutGen.Models;
using Nemeio.LayoutGen.Test.Fakes;
using NUnit.Framework;
using SkiaSharp;
using Key = Nemeio.Core.DataModels.Configurator.Key;

namespace Nemeio.LayoutGen.Test.Modes
{
    [TestFixture]
    public partial class LGClassicLayoutConverterShould
    {
        const string textDisplay    = "A";
        const string imageDisplay   = "emb://windows.svg";

        LGClassicLayoutConverter _converter;
        Font _defaultFont;

        [SetUp]
        public void SetUp()
        {
            var mockDocumentService = Mock.Of<IDocument>();

            Mock.Get(mockDocumentService)
                .Setup(x => x.GetConfiguratorPath())
                .Returns(string.Empty);

            _converter = new LGClassicLayoutConverter(mockDocumentService);
            _defaultFont = FontProvider.GetDefaultFont();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void LGLayoutConverter_Convert_Layout_WorksOk(bool isDark)
        {
            var deviceMap = CreateMap();
            var lgLayout = _converter.Convert(KeyboardModifier.None, new List<Key>(), _defaultFont, isDark, deviceMap, new ImageAdjustment(0, 0));

            var color = isDark ? SKColors.Black : SKColors.White;

            lgLayout.Should().NotBeNull();
            lgLayout.Position.X.Should().Be(0);
            lgLayout.Position.Y.Should().Be(0);
            lgLayout.Size.Should().Be(deviceMap.DeviceSize);
            lgLayout.Keys.Count.Should().Be(0);
            lgLayout.BackgroundColor.Should().Be(color);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void LGLayoutConverter_Convert_SingleKey_WorksOk(bool isDark)
        {
            var singleTextKey = GetKey(textDisplay, KeyDisposition.Single);
            var singleImageKey = GetKey(imageDisplay, KeyDisposition.Single);

            var keys = new List<Key>() { singleTextKey, singleImageKey };
            var deviceMap = CreateMap();
            var lgLayout = _converter.Convert(KeyboardModifier.None, keys, _defaultFont, isDark, deviceMap, new ImageAdjustment(0, 0));

            lgLayout.Keys.Count.Should().Be(keys.Count);

            var renderFirstKey = lgLayout.Keys.ElementAt(0);
            renderFirstKey.GetType().Should().Be(typeof(LGSingleKey));
            renderFirstKey.Subkeys.Count.Should().Be(1);    //  Only None modifier

            var renderSecondKey = lgLayout.Keys.ElementAt(1);
            renderSecondKey.GetType().Should().Be(typeof(LGSingleKey));
            renderSecondKey.Subkeys.Count.Should().Be(1);   //  Only None modifier
        }

        [TestCase(true)]
        [TestCase(false)]
        public void LGLayoutConverter_Convert_DoubleKey_WorksOk(bool isDark)
        {
            var singleTextKey = GetKey(textDisplay, KeyDisposition.Double);
            var singleImageKey = GetKey(imageDisplay, KeyDisposition.Double);

            var keys = new List<Key>() { singleTextKey, singleImageKey };
            var deviceMap = CreateMap();
            var lgLayout = _converter.Convert(KeyboardModifier.None, keys, _defaultFont, isDark, deviceMap, new ImageAdjustment(0, 0));

            lgLayout.Keys.Count.Should().Be(keys.Count);

            var renderFirstKey = lgLayout.Keys.ElementAt(0);
            renderFirstKey.GetType().Should().Be(typeof(LGDoubleKey));
            renderFirstKey.Subkeys.Count.Should().Be(singleTextKey.Actions.Count);

            var renderSecondKey = lgLayout.Keys.ElementAt(1);
            renderSecondKey.GetType().Should().Be(typeof(LGDoubleKey));
            renderSecondKey.Subkeys.Count.Should().Be(singleImageKey.Actions.Count);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void LGLayoutConverter_Convert_FourfoldKey_WorksOk(bool isDark)
        {
            var singleTextKey = GetKey(textDisplay, KeyDisposition.Full);
            var singleImageKey = GetKey(imageDisplay, KeyDisposition.Full);

            var keys = new List<Key>() { singleTextKey, singleImageKey };
            var deviceMap = CreateMap();

            var lgLayout = _converter.Convert(KeyboardModifier.None, keys, _defaultFont, isDark, deviceMap, new ImageAdjustment(0, 0));

            lgLayout.Keys.Count.Should().Be(keys.Count);

            var renderFirstKey = lgLayout.Keys.ElementAt(0);
            renderFirstKey.GetType().Should().Be(typeof(LGFourfoldKey));
            renderFirstKey.Subkeys.Count.Should().Be(singleTextKey.Actions.Count);

            var renderSecondKey = lgLayout.Keys.ElementAt(1);
            renderSecondKey.GetType().Should().Be(typeof(LGFourfoldKey));
            renderSecondKey.Subkeys.Count.Should().Be(singleImageKey.Actions.Count);
        }

        [Test]
        public void LGLayoutConverter_Convert_TextSubKey_WorksOk()
        {
            var singleTextKey = GetKey(textDisplay, KeyDisposition.Single);

            var keys = new List<Key>() { singleTextKey };
            var deviceMap = CreateMap();
            var lgLayout = _converter.Convert(KeyboardModifier.None, keys, _defaultFont, true, deviceMap, new ImageAdjustment(0, 0));

            var renderFirstKey = lgLayout.Keys.ElementAt(0);
            var noneSubKey = renderFirstKey.Subkeys.Where(x => x.Position == LGSubKeyDispositionArea.None).First();
            var textNoneSubKey = noneSubKey as LGTextSubkey;
            textNoneSubKey.Should().NotBeNull();
            textNoneSubKey.Text.Should().Be(textDisplay);
        }

        [Test]
        public void LGLayoutConverter_Convert_ImageSubKey_WorksOk()
        {
            var singleImageKey = GetKey(imageDisplay, KeyDisposition.Single);

            var keys = new List<Key>() { singleImageKey };
            var deviceMap = CreateMap();
            var lgLayout = _converter.Convert(KeyboardModifier.None, keys, _defaultFont, true, deviceMap, new ImageAdjustment(0, 0));

            var renderFirstKey = lgLayout.Keys.ElementAt(0);
            var noneSubKey = renderFirstKey.Subkeys.Where(x => x.Position == LGSubKeyDispositionArea.None).First();
            var imageNoneSubKey = noneSubKey as LGImageSubkey;
            imageNoneSubKey.Should().NotBeNull();
            imageNoneSubKey.Image.Should().Be(imageDisplay);
            imageNoneSubKey.ImageStream.Should().NotBeNull();
        }

        private Key GetKey(string display, KeyDisposition disposition)
        {
            return new Key()
            {
                Disposition = disposition,
                Index = 23,
                Actions = new List<KeyAction>()
                {
                    new KeyAction()
                    {
                        Display = display,
                        Modifier = KeyboardModifier.None
                    },
                    new KeyAction()
                    {
                        Display = "B",
                        Modifier = KeyboardModifier.Shift
                    }
                }
            };
        }

        private NemeioMap CreateMap()
        {
            var mapFactory = new FakeKeyboardNemeioMap();
            var map= mapFactory.CreateEinkMap();
            var deviceMap = new NemeioMap(map);

            return deviceMap;
        }
    }
}
