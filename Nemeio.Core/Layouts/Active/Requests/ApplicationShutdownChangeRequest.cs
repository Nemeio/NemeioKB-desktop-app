using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Layouts.Active.Historic;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;

namespace Nemeio.Core.Layouts.Active.Requests
{
    public sealed class ApplicationShutdownChangeRequest : EntityLayoutChangeRequest
    {
        public ApplicationShutdownChangeRequest(ILoggerFactory loggerFactory, ISystem system, ILayoutLibrary library, INemeio nemeio) 
            : base(EntityRequestId.Ui,loggerFactory,  system, library, nemeio) { }

        public override async Task<ILayout> ApplyAsync(ILayoutHolderNemeioProxy proxy, IActiveLayoutHistoric historic, ILayout lastSynchronized)
        {
            ILayout syncLayout = null;

            if (lastSynchronized != null)
            {
                syncLayout = _library
                    .Layouts
                    .Where(x => x.LayoutInfo.Hid)
                    .Where(x => x.LayoutId == lastSynchronized.AssociatedLayoutId)
                    .FirstOrDefault();

                if (syncLayout != null)
                {
                    await ApplySystemLayout(syncLayout);

                    if (proxy != null)
                    {
                        await ApplyKeyboardLayout(proxy, syncLayout);
                    }
                }
            }

            if (syncLayout != null)
            {
                var historicLog = new HistoricLog(syncLayout, HistoricActor.User);

                historic.Insert(historicLog);
            }

            return syncLayout;
        }
    }
}
