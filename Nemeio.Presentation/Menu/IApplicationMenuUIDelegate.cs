using System.Threading.Tasks;
using Nemeio.Core.DataModels.Locale;

namespace Nemeio.Presentation.Menu
{
    public interface IApplicationMenuUIDelegate
    {
        Task DisplayDialogAsync(StringId title, StringId message);
        Task DisplayNotificationAsync(StringId title, StringId message);
    }
}
