using System;
using Nemeio.Core.Managers;
using Nemeio.Core.PackageUpdater;
using Nemeio.Mac.Windows.Alert.UpdateModal;
using Nemeio.Presentation.PackageUpdater.UI;

namespace Nemeio.Mac.Modals
{
    public class MacUpdateModal : MacModalWindow<UpdateModalController>
    {
        private readonly IPackageUpdater _packageUpdater;
        private readonly IPackageUpdaterMessageFactory _messageFactory;

        public MacUpdateModal(ILanguageManager languageManager, IPackageUpdater packageUpdate, IPackageUpdaterMessageFactory messageFactory)
            : base(languageManager)
        {
            _packageUpdater = packageUpdate ?? throw new ArgumentNullException(nameof(packageUpdate));
            _messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
        }

        public override UpdateModalController CreateNativeModal()
        {
            return UpdateModalController.Create(_packageUpdater, _languageManager, _messageFactory, () =>
            {
                OnClose();
            });
        }
    }
}
