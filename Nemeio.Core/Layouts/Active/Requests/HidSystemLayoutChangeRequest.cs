using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Layouts.Active.Historic;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;

namespace Nemeio.Core.Layouts.Active.Requests
{
    public sealed class HidSystemLayoutChangeRequest : EntityLayoutChangeRequest
    {
        public HidSystemLayoutChangeRequest(ILoggerFactory loggerFactory, ISystem system, ILayoutLibrary library, INemeio nemeio) 
            : base(EntityRequestId.System, loggerFactory, system, library, nemeio) { }

        public override async Task<ILayout> ApplyAsync(ILayoutHolderNemeioProxy proxy, IActiveLayoutHistoric historic, ILayout lastSynchronized)
        {
            ILayout syncLayout = null;

            if (proxy != null)
            {
                syncLayout = _library.Layouts.FirstOrDefault(x => x.LayoutInfo.OsLayoutId.ToString() == _system.SelectedLayout.ToString());
                
                if (syncLayout != null)
                {
                    await ApplyKeyboardLayout(proxy, syncLayout);

                    var log = new HistoricLog(syncLayout, HistoricActor.System);

                    historic.Insert(log);
                }
            }

            return syncLayout;
        }
    }
}
