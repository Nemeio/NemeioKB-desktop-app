using System.Threading.Tasks;
using Nemeio.Core.Layouts.Active.Historic;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Layouts.Active.Requests.Base
{
    public interface IChangeRequest
    {
        Task<ILayout> ApplyLayoutAsync(IActiveLayoutHistoric historic, ILayout lastSynchronized);
        Task ExecuteAsync();
    }
}
