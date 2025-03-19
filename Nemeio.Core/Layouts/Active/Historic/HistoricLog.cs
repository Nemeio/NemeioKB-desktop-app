using System;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Layouts.Active.Historic
{
    public sealed class HistoricLog : IHistoricLog
    {
        public ILayout Layout { get; private set; }
        public HistoricActor Actor { get; private set; }

        public HistoricLog(ILayout layout, HistoricActor actor)
        {
            Layout = layout ?? throw new ArgumentNullException(nameof(layout));
            Actor = actor;
        }
    }
}
