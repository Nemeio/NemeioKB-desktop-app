using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Systems
{
    public interface ISystemLayoutInteractor
    {
        void ChangeSelectedLayout(OsLayoutId layoutid);
    }
}
