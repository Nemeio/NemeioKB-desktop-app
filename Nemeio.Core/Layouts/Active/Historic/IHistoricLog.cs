using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Layouts.Active.Historic
{
    public interface IHistoricLog
    {
        ILayout Layout { get; }
        HistoricActor Actor { get; }
    }
}
