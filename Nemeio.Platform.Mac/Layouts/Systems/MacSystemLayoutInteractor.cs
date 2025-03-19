using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;
using Nemeio.Mac.Native;

namespace Nemeio.Platform.Mac.Layouts.Systems
{
    public class MacSystemLayoutInteractor : ISystemLayoutInteractor
    {
        public void ChangeSelectedLayout(OsLayoutId layoutid)
        {
            ExtendedTools.SetCurrentKeyboardLayout(layoutid);
        }
    }
}
