using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Errors;
using Nemeio.Core.Services;
using Nemeio.Models.Fonts;

namespace Nemeio.Core.Models.Fonts
{
    public class FontProvider : IFontProvider
    {
        private const string ResourcesPath = "Nemeio.Core.Resources";
        private const string FontsFilePath = "fonts.xml";
        private const int FontWantedCount = 8;

        private readonly ILogger _logger;
        private readonly HashSet<FontInfo> _fonts;
        private readonly IErrorManager _errorManager;
        private readonly IDocument _documentService;

        public FontProvider(ILoggerFactory loggerFactory, IErrorManager errorManager, IDocument documentService = null)
        {
            _fonts = new HashSet<FontInfo>();
            _logger = loggerFactory.CreateLogger<FontProvider>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            _errorManager = errorManager ?? throw new ArgumentNullException(nameof(errorManager));
            _documentService = documentService;
            LoadFonts();
        }

        public HashSet<FontInfo> Fonts
        {
            get { return _fonts; }
        }

        public static Font GetDefaultFont() => new Font(NemeioConstants.Noto, FontSize.Medium, false, false);

        public bool RegisterFont(FontInfo font)
        {
            if (font == null)
            {
                throw new ArgumentNullException(nameof(font));
            }

            if (_fonts.Any(x => x.Priority == font.Priority))
            {
                var errorMessage = $"Priority value <{font.Priority}> already used.";

                _logger.LogError(errorMessage);

                throw new InvalidDataException(errorMessage);
            }

            var registered = _fonts.Add(font);
            if (registered)
            {
                _logger.LogInformation($"Font <{font.Name}> registered with priority index <{font.Priority}>");
            }

            return registered;
        }

        public bool FontExists(string fontName) => _fonts.Select(x => x.Name).Contains(fontName);

        private void LoadFonts()
        {
            if (_documentService != null && File.Exists(OverrideFontFileFullPath()))
            {
                LoadOverriddenFonts();
            }
            else
            {
                LoadEmbeddedFonts();
            }
        }
        private void LoadEmbeddedFonts()
        {
            if (!ResourceExists(FontsFilePath))
            {
                throw new MissingMemberException($"{_errorManager.GetFullErrorMessage(ErrorCode.CoreLoadRequiredFontFailed)}.");
            }
            using (var stream = GetResourceStream(FontsFilePath))
            {
                LoadFontsFromStream(GetResourceStream(FontsFilePath));
            }
        }
        private void LoadOverriddenFonts()
        {
            using (var stream = new FileStream(OverrideFontFileFullPath(), FileMode.Open))
            {
                LoadFontsFromStream(stream);
            }
        }

        private void LoadFontsFromStream(Stream stream)
        {
            var xmlSerializer = new XmlSerializer(typeof(FontsInfo));
            var loadedFonts = (FontsInfo)xmlSerializer.Deserialize(stream);

            if (loadedFonts == null)
            {
                throw new MissingMemberException($"{_errorManager.GetFullErrorMessage(ErrorCode.CoreLoadRequiredFontFailed)}.");
            }

            if (loadedFonts.Fonts.Count() < FontWantedCount || loadedFonts.Fonts.Count() > FontWantedCount)
            {
                throw new MissingMemberException($"{_errorManager.GetFullErrorMessage(ErrorCode.CoreLoadRequiredFontFailed)}.");
            }

            foreach (var font in loadedFonts.Fonts)
            {
                RegisterFont(font);
            }
        }
        private bool ResourceExists(string resourceName)
        {
            var resourceNames = GetCoreAssembly().GetManifestResourceNames();
            var fileName = $"{ResourcesPath}.{resourceName}";

            return resourceNames.Contains(fileName);
        }



        public virtual Stream GetResourceStream(string filename) => GetCoreAssembly().GetManifestResourceStream($"{ResourcesPath}.{filename}");

        private Assembly GetCoreAssembly() => Assembly.GetAssembly(typeof(IFontProvider));

        private string OverrideFontFileFullPath() => Path.Combine(_documentService.DocumentPath, FontsFilePath);

        public void RefreshFonts()
        {
            Fonts.Clear();
            LoadFonts();
        }
    }
}
