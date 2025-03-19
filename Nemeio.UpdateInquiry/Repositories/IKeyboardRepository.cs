using System.Collections.Generic;
using System.Threading.Tasks;
using Nemeio.UpdateInquiry.Models;

namespace Nemeio.UpdateInquiry.Repositories
{
    public interface IKeyboardRepository
    {
        Task<IEnumerable<Keyboard>> GetKeyboards();
    }
}
