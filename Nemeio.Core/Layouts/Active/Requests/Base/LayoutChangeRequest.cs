using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Layouts.Active.Historic;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;

namespace Nemeio.Core.Layouts.Active.Requests
{
    public abstract class LayoutChangeRequest : EntityLayoutChangeRequest
    {
        public ILayout Layout { get; private set; }

        protected LayoutChangeRequest(EntityRequestId id, ILayout layout, ILoggerFactory loggerFactory, ISystem system, ILayoutLibrary library, INemeio nemeio)
            : base(id,loggerFactory, system, library, nemeio)
        {
            Layout = layout ?? throw new ArgumentNullException(nameof(layout));
        }

        public override async Task<ILayout> ApplyAsync(ILayoutHolderNemeioProxy proxy, IActiveLayoutHistoric historic, ILayout lastSynchronized)
        {
            await ApplySystemLayout(Layout);

            if (proxy != null)
            {
                await ApplyKeyboardLayout(proxy, Layout);
            }

            var log = new HistoricLog(Layout, HistoricActor.User);

            historic.Insert(log);

            return Layout;
        }
    }
}
