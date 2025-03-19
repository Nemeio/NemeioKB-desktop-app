using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Layouts.Active.Historic;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;

namespace Nemeio.Core.Layouts.Active.Requests
{
    public sealed class HistoricChangeRequest : EntityLayoutChangeRequest
    {
        public bool IsBack { get; private set; }

        public HistoricChangeRequest(bool isBack,ILoggerFactory loggerFactory, ISystem system, ILayoutLibrary library, INemeio nemeio) 
            : base(EntityRequestId.Keyboard,loggerFactory,  system, library, nemeio) 
        {
            IsBack = isBack;
        }

        public override async Task<ILayout> ApplyAsync(ILayoutHolderNemeioProxy proxy, IActiveLayoutHistoric historic, ILayout lastSynchronized)
        {
            var log = IsBack ? historic.Back() : historic.Forward();
            var syncLayout = log.Layout;

            if (syncLayout != null)
            {
                await ApplySystemLayout(syncLayout);

                if (proxy != null)
                {
                    await ApplyKeyboardLayout(proxy, syncLayout);
                }
            }

            return syncLayout;
        }
    }
}
