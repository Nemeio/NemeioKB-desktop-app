using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Layouts.Active.Historic;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;
using Nemeio.Core.Systems.Sessions;

namespace Nemeio.Core.Layouts.Active.Requests
{
    public sealed class SessionStateChangeRequest : EntityLayoutChangeRequest
    {
        public SessionState State { get; private set; }

        public SessionStateChangeRequest(SessionState state,ILoggerFactory loggerFactory, ISystem system, ILayoutLibrary library, INemeio nemeio) 
            : base(EntityRequestId.System,loggerFactory, system, library, nemeio) 
        {
            State = state;
        }

        public override async Task<ILayout> ApplyAsync(ILayoutHolderNemeioProxy proxy, IActiveLayoutHistoric historic, ILayout lastSynchronized)
        {
            ILayout syncLayout = null;

            switch (State)
            {
                case SessionState.Lock:
                    syncLayout = await SessionLockStategy(proxy, lastSynchronized, historic);
                    break;
                case SessionState.Open:
                    syncLayout = await SessionUnlockStrategy(lastSynchronized, historic);
                    break;
                default:
                    //  Case is not supported
                    //  We do nothing
                    break;
            }

            return syncLayout;
        }

        private async Task<ILayout> SessionLockStategy(ILayoutHolderNemeioProxy proxy, ILayout lastSynchronized, IActiveLayoutHistoric historic)
        {
            ILayout selectedLayout = null;

            if (proxy != null)
            {
                selectedLayout = _library.Layouts.FirstOrDefault(lyt => lyt.LayoutId == proxy.SelectedLayoutId);
                if (selectedLayout != null)
                {
                    var defaultSystemOsLayout = _system.DefaultLayout;
                    var defaultSystemLayout = _library.Layouts.FirstOrDefault(lyt => lyt.LayoutInfo.OsLayoutId.ToString() == defaultSystemOsLayout.ToString());
                    if (defaultSystemLayout != null)
                    {
                        await ApplyKeyboardLayout(proxy, defaultSystemLayout);
                    }

                    await ApplySystemLayout(defaultSystemOsLayout);
                }
            }

            if (selectedLayout != null)
            {
                var log = new HistoricLog(selectedLayout, HistoricActor.System);

                historic.Insert(log);
            }

            return selectedLayout;
        }

        private async Task<ILayout> SessionUnlockStrategy(ILayout lastSynchronized, IActiveLayoutHistoric historic)
        {
            ILayout selectedLayout = null;

            if (lastSynchronized != null)
            {
                var layout = _library
                    .Layouts
                    .FirstOrDefault(x => x.LayoutId == lastSynchronized.LayoutId);

                if (layout != null)
                {
                    await ApplySystemLayout(layout);

                    selectedLayout = layout;
                }
            }

            if (selectedLayout != null)
            {
                var log = new HistoricLog(selectedLayout, HistoricActor.System);

                historic.Insert(log);
            }

            return selectedLayout;
        }
    }
}
