using System.Threading.Tasks;
using Nemeio.Core.Layouts.Export;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Layouts
{
    public interface ILayoutFacade
    {
        Task AddLayoutAsync(ILayout layout);
        Task UpdateLayoutAsync(ILayout layout);
        Task RemoveLayoutAsync(ILayout layout);
        Task RemoveLayoutAsync(LayoutId layoutId);
        Task<ILayout> DuplicateLayoutAsync(ILayout layout, string title);
        Task<LayoutExport> ExportLayoutAsync(ILayout layout);
        Task<ILayout> ImportLayoutAsync(LayoutExport layout);
        Task RefreshAugmentedLayoutAsync();
        Task SetDefaultLayoutAsync(LayoutId id);
        Task ResetDefaultLayoutAsync();
    }
}
