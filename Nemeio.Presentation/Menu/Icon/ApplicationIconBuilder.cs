using System;
using Nemeio.Core;
using Nemeio.Core.Icon;
using Nemeio.Core.Managers;
using Nemeio.Core.Services.Batteries;
using Nemeio.Presentation.PackageUpdater.UI;

namespace Nemeio.Presentation.Menu.Icon
{
    public sealed class ApplicationIconBuilder
    {
        private readonly ILanguageManager _languageManager;
        private readonly IPackageUpdaterMessageFactory _messageFactory;

        public ApplicationIconBuilder(ILanguageManager languageManager, IPackageUpdaterMessageFactory messageFactory)
        {
            _languageManager = languageManager ?? throw new ArgumentNullException(nameof(languageManager));
            _messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
        }

        public ApplicationIcon Build(ApplicationIconType iconType)
        {
            return new ApplicationIcon(iconType);
        }
    }
}
