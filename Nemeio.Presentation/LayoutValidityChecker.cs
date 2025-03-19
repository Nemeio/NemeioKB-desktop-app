using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Map;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Services;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Presentation
{
    public class LayoutValidityChecker : ILayoutValidityChecker
    {
        private readonly ILogger _logger;
        private readonly IKeyboardMapFactory _keyboardMapFactory;

        public LayoutValidityChecker(ILoggerFactory loggerFactory, IKeyboardMapFactory keyboardMapFactory)
        {
            _logger = loggerFactory.CreateLogger<LayoutValidityChecker>();
            _keyboardMapFactory = keyboardMapFactory ?? throw new ArgumentNullException(nameof(keyboardMapFactory));
        }

        public bool Check(ILayout layout)
        {
            try
            {
                if (layout.LayoutInfo.Hid && string.IsNullOrWhiteSpace(layout.LayoutInfo.OsLayoutId))
                {
                    throw new InvalidOperationException("Os layout id is null or empty on HID layout");
                }

                if (string.IsNullOrWhiteSpace(layout.Title))
                {
                    throw new InvalidOperationException("Invalid title");
                }

                if (!layout.Image.Any(x => x != 0))
                {
                    throw new InvalidOperationException("Blank image invalid");
                }

                if (layout.Keys == null)
                {
                    throw new InvalidOperationException("Layout has no keys");
                }

                var keysCount = layout.LayoutImageInfo.Screen.Map.Buttons.Count;

                if (layout.Keys.Count() < keysCount || layout.Keys.Count() > keysCount)
                {
                    throw new InvalidOperationException("Number of keys invalid");
                }

                return true;
            }
            catch (InvalidOperationException exception)
            {
                _logger.LogError(exception, "Invalid value");

                return false;
            }
        }
    }
}
