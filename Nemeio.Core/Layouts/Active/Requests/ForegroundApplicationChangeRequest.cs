using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Keyboard.Nemeios.Runner;
using Nemeio.Core.Layouts.Active.Historic;
using Nemeio.Core.Layouts.LinkedApplications;
using Nemeio.Core.Managers;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;
using Nemeio.Core.Systems.Applications;

namespace Nemeio.Core.Layouts.Active.Requests
{
    public sealed class ForegroundApplicationChangeRequest : EntityLayoutChangeRequest
    {
        private readonly ILanguageManager _languageManager;
        private readonly IApplicationLayoutManager _applicationLayoutManager;
        private readonly NemeioRunner _nemeioRunner;

        public Application Application { get; private set; }


        public ForegroundApplicationChangeRequest(Application application, ILoggerFactory loggerFactory, ISystem system, ILayoutLibrary library, INemeio nemeio, ILanguageManager languageManager, IApplicationLayoutManager applicationLayoutManager)
            : base(EntityRequestId.Ui, loggerFactory, system, library, nemeio)
        {
            Application = application ?? throw new ArgumentNullException(nameof(application));

            _languageManager = languageManager ?? throw new ArgumentNullException(nameof(languageManager));
            _applicationLayoutManager = applicationLayoutManager ?? throw new ArgumentNullException(nameof(applicationLayoutManager));
            _nemeioRunner = nemeio as NemeioRunner;
        }

        public override async Task<ILayout> ApplyAsync(ILayoutHolderNemeioProxy proxy, IActiveLayoutHistoric historic, ILayout lastSynchronized)
        {
            ILayout selectedLayout = null;

            if (!string.IsNullOrWhiteSpace(Application.ApplicationPath) && _applicationLayoutManager.FindException(Application) == null)
            {
                var targetLayoutId = _applicationLayoutManager.FindLatestAssociatedLayoutId(Application);
                if (targetLayoutId == null || IsConfiguratorWindow(Application))
                {
                    //  No layout found
                    //  We retrieve last layout selected by user

                    targetLayoutId = historic
                        .Historic
                        .Reverse()
                        .FirstOrDefault(x => x.Actor != HistoricActor.Application)?.Layout.LayoutId;

                }

                if (targetLayoutId != null && targetLayoutId != _nemeioRunner?.SelectedLayoutId)
                {
                    var targetLayout = _library.Layouts.FirstOrDefault(x => x.LayoutId == targetLayoutId);
                    if (targetLayout != null && proxy != null && targetLayout.Enable)
                    {
                        await ApplySystemLayout(targetLayout);                        
                        await ApplyKeyboardLayout(proxy, targetLayout);
                    }
                    selectedLayout = _library.Layouts.FirstOrDefault(x => x.LayoutId == targetLayoutId);
                }
            }

            if (selectedLayout != null)
            {
                var log = new HistoricLog(selectedLayout, HistoricActor.Application);

                historic.Insert(log);
            }

            return selectedLayout;
        }

        /// <summary>
        /// WARNING! Today the only way to know if the current selected windows is configurator's window is it's title with the associated program's name
        /// </summary>
        private bool IsConfiguratorWindow(Application application)
        {
            if (application == null)
            {
                return false;
            }

            if (application.WindowTitle != null && application.Name != null)
            {
                var configuratorWindowTitle = _languageManager.GetLocalizedValue(StringId.ConfiguratorTitle);
                var isConfigurator = application.WindowTitle.Equals(configuratorWindowTitle, StringComparison.OrdinalIgnoreCase) &&
                                    application.Name.Equals(NemeioConstants.AppName, StringComparison.OrdinalIgnoreCase);

                return isConfigurator;
            }

            return false;
        }
    }
}
