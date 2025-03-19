using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Applications;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Errors;
using Nemeio.Core.Images;
using Nemeio.Core.Images.Jpeg.Builder;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Layouts.Color;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Layouts.Images.AugmentedLayout;
using Nemeio.Core.Managers;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Services;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Settings;
using Nemeio.Core.Settings.Types;
using Nemeio.LayoutGen;
using Nemeio.Platform.Windows.Layouts;
using Nemeio.Platform.Windows.Layouts.Images;
using Nemeio.Windows.Application.Resources;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Nemeio.Platform.Windows.Tests
{
    /// <summary>
    /// These test required to be launched on x64.
    /// Test ->  processor architecture -> x64
    /// </summary>
    [TestFixture]
    public class WinLayoutImageGeneratorShould
    {
        private static readonly IntPtr AzertyHandle = (IntPtr)0x00000000040c040c;
        private static readonly IntPtr QwertyHandle = (IntPtr)0x0000000004090409;

        private WinLayoutImageGenerator _genService;
        private IFontProvider _fontProvider;
        private InputLanguage _azerty;
        private InputLanguage _qwerty;

        private IScreen _holitechScreen;
        private IScreen _einkScreen;

        [SetUp]
        public void SetUp()
        {
            var loggerFactory = new LoggerFactory();
            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            var mockAugmentedLayoutImageProvider = Mock.Of<IAugmentedLayoutImageProvider>();
            var mockLanguageManager = Mock.Of<ILanguageManager>();
            var mockDocument = Mock.Of<IDocument>();
            var mockSettingsHolder = Mock.Of<ISettingsHolder>();

            //  Layout tests are based on image comparaison
            //  All layouts generated has been done with a 40% jpeg compression level
            //  We must force this value to avoid side effect when we will change default jpeg compression level
            const int NeededJpegCompressionLevel = 40;

            var mockLogger = Mock.Of<ILogger>();
            var mockSettings = new Settings(
                new SwaggerEnableSetting(mockLogger, null),
                new ApiPortSetting(mockLogger, null),
                new EnvironmentSetting(mockLogger, null),
                new JpegCompressionLevelSetting(mockLogger, NeededJpegCompressionLevel),
                new AutoStartWebServerSetting(mockLogger, null)
            );

            Mock.Get(mockSettingsHolder)
                .Setup(x => x.Settings)
                .Returns(mockSettings);

            var jpegImagePackageBuilder = new JpegImagePackageBuilder();
            var osLayoutBuilder = new WinOsLayoutIdBuilder(mockLanguageManager);

            var keyboardMapFactory = new WinKeyboardMapFactory();
            _fontProvider = new FontProvider(loggerFactory, new ErrorManager());

            var jpegRendererStrategy = Registerer.CreateJpegRenderer(loggerFactory, mockDocument, _fontProvider, jpegImagePackageBuilder);
            var oneBppRendererStrategy = Registerer.CreateOneBppRenderer(loggerFactory, mockDocument, _fontProvider);

            var screenFactory = new ScreenFactory(keyboardMapFactory, jpegRendererStrategy, oneBppRendererStrategy, mockSettingsHolder);
            var resourceLoader = new WinResourceLoader();

            _holitechScreen = screenFactory.CreateHolitechScreen();
            _einkScreen = screenFactory.CreateEinkScreen();

            _genService = new WinLayoutImageGenerator(_fontProvider, mockApplicationSettingsManager, mockAugmentedLayoutImageProvider, osLayoutBuilder, resourceLoader);

            foreach (InputLanguage inpt in InputLanguage.InstalledInputLanguages)
            {
                if (inpt.Handle == AzertyHandle)
                {
                    _azerty = inpt;
                }
                else if (inpt.Handle == QwertyHandle)
                {
                    _qwerty = inpt;
                }
            }
        }

        [Test]
        public void WinLayoutGenService_WhenScreenIsHolitech_RenderAzertyKeysFromSystem()
        {
            if (PipelineChecker.RunningOnPipeline())
            {
                return;
            }

            CompareKeysFromSystemLayout(GetLayoutId(_azerty), _holitechScreen, "Holitech.azerty.ldlc").Should().BeTrue();
        }

        [Test]
        public void WinLayoutGenService_WhenScreenIsHolitech_RenderQwertyKeysFromSystem()
        {
            if (PipelineChecker.RunningOnPipeline())
            {
                return;
            }

            CompareKeysFromSystemLayout(GetLayoutId(_qwerty), _holitechScreen, "Holitech.qwerty.ldlc").Should().BeTrue();
        }

        [Test]
        public void WinLayoutGenService_WhenScreenIsEink_RenderAzertyKeysFromSystem()
        {
            if (PipelineChecker.RunningOnPipeline())
            {
                return;
            }

            CompareKeysFromSystemLayout(GetLayoutId(_azerty), _einkScreen, "Eink.azerty.ldlc").Should().BeTrue();
        }

        [Test]
        public void WinLayoutGenService_WhenScreenIsEink_RenderQwertyKeysFromSystem()
        {
            if (PipelineChecker.RunningOnPipeline())
            {
                return;
            }

            CompareKeysFromSystemLayout(GetLayoutId(_qwerty), _einkScreen, "Eink.qwerty.ldlc").Should().BeTrue();
        }

        [TestCase("Holitech.azerty.ldlc")]
        [TestCase("Holitech.qwerty.ldlc")]
        [TestCase("Holitech.arabic.ldlc")]
        [TestCase("Holitech.german.ldlc")]
        [TestCase("Holitech.italian.ldlc")]
        [TestCase("Holitech.japanese.ldlc")]
        [TestCase("Holitech.korean.ldlc")]
        [TestCase("Holitech.russian.ldlc")]
        [TestCase("Holitech.spanish.ldlc")]
        [TestCase("Holitech.united-kingdom.ldlc")]
        [TestCase("Holitech.buryat")]
        public void WinLayoutGenService_WhenScreenIsHolitech_RenderWallpaperFromJson(string languageName)
        {
            CompareWallpaperFromJson(languageName, _holitechScreen).Should().BeTrue();
        }

        [TestCase("Eink.azerty.ldlc")]
        [TestCase("Eink.qwerty.ldlc")]
        [TestCase("Eink.arabic.ldlc")]
        [TestCase("Eink.german.ldlc")]
        [TestCase("Eink.italian.ldlc")]
        [TestCase("Eink.japanese.ldlc")]
        [TestCase("Eink.korean.ldlc")]
        [TestCase("Eink.russian.ldlc")]
        [TestCase("Eink.spanish.ldlc")]
        [TestCase("Eink.united-kingdom.ldlc")]
        public void WinLayoutGenService_WhenScreenIsEink_RenderWallpaperFromJson(string languageName)
        {
            CompareWallpaperFromJson(languageName, _einkScreen).Should().BeTrue();
        }

        [TestCase(LayoutImageType.Bold)]
        [TestCase(LayoutImageType.Gray)]
        [TestCase(LayoutImageType.Hide)]
        public void WinLayoutGenService_WhenScreenIsHolitech_RenderWallpaperFromJsonWithType(LayoutImageType imageType)
        {
            const string languageName = "azerty.ldlc";

            var json = FileContentFromResources($"Holitech.{languageName}.layout.json");
            var wallpaper = ByteArrayFromResources($"Holitech.ImageTypes.{languageName}_{imageType}.wallpaper");

            var keys = JsonConvert.DeserializeObject<IEnumerable<Key>>(json);
            var layoutInfo = new LayoutInfo(
                OsLayoutId.Empty,
                isFactory: false,
                isHid: true,
                augmentedHidEnable: false
            );
            var layoutImageInfo = new LayoutImageInfo(
                HexColor.Black,
                font: FontProvider.GetDefaultFont(),
                _holitechScreen
            );
            var imageRequest = new ImageRequest(layoutInfo, layoutImageInfo, keys, _holitechScreen, new ImageAdjustment(0, 0));

            var layout = _genService.RenderLayoutImage(imageRequest);

            layout.Should().BeEquivalentTo(wallpaper);
        }

        [TestCase(LayoutImageType.Bold)]
        [TestCase(LayoutImageType.Gray)]
        [TestCase(LayoutImageType.Hide)]
        public void WinLayoutGenService_WhenScreenIsEink_RenderWallpaperFromJsonWithType(LayoutImageType imageType)
        {
            const string languageName = "azerty.ldlc";

            var json = FileContentFromResources($"Eink.{languageName}.layout.json");
            var wallpaper = ByteArrayFromResources($"Eink.ImageTypes.{languageName}_{imageType}.wallpaper");

            var keys = JsonConvert.DeserializeObject<IEnumerable<Key>>(json);
            var layoutInfo = new LayoutInfo(
                OsLayoutId.Empty,
                isFactory: false,
                isHid: true,
                augmentedHidEnable: false
            );
            var layoutImageInfo = new LayoutImageInfo(
                HexColor.Black,
                font: FontProvider.GetDefaultFont(),
                _einkScreen
            );
            var imageRequest = new ImageRequest(layoutInfo, layoutImageInfo, keys, _einkScreen, new ImageAdjustment(0, 0));

            var layout = _genService.RenderLayoutImage(imageRequest);

            layout.Should().BeEquivalentTo(wallpaper);
        }

        [Test]
        public void WinLayoutGenService_WhenScreenIsEink_RenderWallpaperFromJson_WithGrayLetters()
        {
            const string languageName = "azerty.ldlc.gray";

            var json = FileContentFromResources($"Eink.{languageName}.layout.json");
            var wallpaper = ByteArrayFromResources($"Eink.{languageName}.wallpaper");

            var keys = JsonConvert.DeserializeObject<IEnumerable<Key>>(json);
            var layoutInfo = new LayoutInfo(
                OsLayoutId.Empty,
                isFactory: false,
                isHid: true,
                augmentedHidEnable: false
            );
            var layoutImageInfo = new LayoutImageInfo(
                HexColor.Black,
                font: FontProvider.GetDefaultFont(),
                _einkScreen
            );
            var imageRequest = new ImageRequest(layoutInfo, layoutImageInfo, keys, _einkScreen, new ImageAdjustment(0, 0));

            var layout = _genService.RenderLayoutImage(imageRequest);

            layout.Should().BeEquivalentTo(wallpaper);
        }

        private OsLayoutId GetLayoutId(InputLanguage inpt) => new WinOsLayoutId(inpt.Culture.Name, inpt.Handle);

        private bool CompareKeysFromSystemLayout(OsLayoutId lyt, IScreen screen, string name)
        {
            var systemKeys = _genService.CreateLayoutKeys(screen, lyt);
            var testableSystemKeys = JsonConvert.SerializeObject(systemKeys);

            var sampleKeys = FileContentFromResources($"{name}.layout.json");

            return sampleKeys.Equals(testableSystemKeys);
        }

        private bool CompareWallpaperFromJson(string languageName, IScreen screen)
        {
            var json = FileContentFromResources($"{languageName}.layout.json");
            var wallpaper = ByteArrayFromResources($"{languageName}.wallpaper");

            var keys = JsonConvert.DeserializeObject<IEnumerable<Key>>(json);

            var layoutInfo = new LayoutInfo(OsLayoutId.Empty, false, false);
            var layoutImageInfo = new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), screen, LayoutImageType.Classic);
            var imageRequest = new ImageRequest(layoutInfo, layoutImageInfo, keys, screen, new ImageAdjustment(0, 0));

            var layout = _genService.RenderLayoutImage(imageRequest);

            return layout.SequenceEqual(wallpaper);
        }

        private byte[] ByteArrayFromResources(string name)
        {
            using (var stream = GetResourceStream(name))
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);

                return memoryStream.ToArray();
            }
        }

        private string FileContentFromResources(string name)
        {
            using (var stream = GetResourceStream(name))
            using (var fileStream = new StreamReader(stream))
            {
                return fileStream.ReadToEnd();
            }
        }

        private Stream GetResourceStream(string name)
        {
            var asm = Assembly.GetExecutingAssembly();
            var resource = GetResourceFullName(name);

            return asm.GetManifestResourceStream(resource);
        }

        private string GetResourceFullName(string name) => string.Format("Nemeio.Platform.Windows.Tests.Resources.{0}", name);
    }
}
