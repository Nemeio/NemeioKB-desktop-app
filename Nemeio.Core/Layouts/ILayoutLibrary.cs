using System.Collections.Generic;
using System.Threading.Tasks;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Tools;

namespace Nemeio.Core.Layouts
{
    public interface ILayoutLibrary : IStoppable
    {
        IList<ILayout> Layouts { get; }

        void Start(IScreen screen);

        Task<ILayout> AddLayoutAsync(ILayout layout);
        Task<ILayout> UpdateLayoutAsync(ILayout layout);
        Task RemoveLayoutAsync(ILayout layout);
        Task RemoveLayoutAsync(LayoutId layoutId);
    }
}
