using System.Collections.Generic;
using System.Threading.Tasks;
using Nemeio.Core.JsonModels;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Keys
{
    public interface IKeyHandler
    {
        Task HandleAsync(INemeio nemeio, LayoutId id, IList<NemeioIndexKeystroke> keystrokes);
    }
}
