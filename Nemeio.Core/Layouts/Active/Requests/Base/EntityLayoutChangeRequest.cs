using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Layouts.Active.Requests.Base;
using Nemeio.Core.Systems;

namespace Nemeio.Core.Layouts.Active.Requests
{
    public abstract class EntityLayoutChangeRequest : ChangeRequest
    {
        public EntityRequestId Id { get; private set; }

        protected EntityLayoutChangeRequest(EntityRequestId id,ILoggerFactory loggerFactory, ISystem system, ILayoutLibrary library, INemeio nemeio)
            : base(loggerFactory, system,  library, nemeio)
        {
            Id = id;
        }
    }
}
