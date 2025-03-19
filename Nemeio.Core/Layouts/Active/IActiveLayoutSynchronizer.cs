using System;
using System.Threading.Tasks;
using Nemeio.Core.Layouts.Active.Requests.Base;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Layouts.Active
{
    public interface IActiveLayoutSynchronizer
    {
        bool Syncing { get; }
        ILayout LastSynchronizedLayout { get; }

        event EventHandler ActiveLayoutChanged;
        Task PostRequestAsync(IChangeRequest request);
        void ResetHistoric();
    }
}
