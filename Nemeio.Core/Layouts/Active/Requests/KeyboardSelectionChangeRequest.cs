using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Layouts.Active.Historic;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;

namespace Nemeio.Core.Layouts.Active.Requests
{
    public sealed class KeyboardSelectionChangeRequest : EntityLayoutChangeRequest
    {
        public KeyboardSelectionChangeRequest(ILoggerFactory loggerFactory, ISystem system, ILayoutLibrary library, INemeio nemeio) 
            : base(EntityRequestId.Keyboard,loggerFactory, system, library, nemeio) { }

        public override async Task<ILayout> ApplyAsync(ILayoutHolderNemeioProxy proxy, IActiveLayoutHistoric historic, ILayout lastSynchronized)
        {
            ILayout syncLayout = null;

            if (proxy != null)
            {
                syncLayout = _library.Layouts.FirstOrDefault(x => x.LayoutId == proxy.SelectedLayoutId);
                if (syncLayout != null)
                {
                    await ApplySystemLayout(syncLayout);

                    var log = new HistoricLog(syncLayout, HistoricActor.User);

                    historic.Insert(log);
                }
            }

            return syncLayout;
        }
    }
}
