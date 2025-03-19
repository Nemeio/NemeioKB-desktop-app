using System.Collections.Generic;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Layouts.Active.Historic
{
    public interface IActiveLayoutHistoric
    {
        uint MaxItemCount { get; }
        int Index { get; }
        IList<IHistoricLog> Historic { get; }
        IHistoricLog Back();
        IHistoricLog Forward();
        void Insert(IHistoricLog layout);
        void Remove(ILayout layout);
        void Reset();
    }
}
