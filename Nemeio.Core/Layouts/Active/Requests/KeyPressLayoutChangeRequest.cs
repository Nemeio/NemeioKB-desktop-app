using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;

namespace Nemeio.Core.Layouts.Active.Requests
{
    public sealed class KeyPressLayoutChangeRequest : LayoutChangeRequest
    {
        public KeyPressLayoutChangeRequest(ILayout layout, ILoggerFactory loggerFactory, ISystem system, ILayoutLibrary library, INemeio nemeio)
            : base(EntityRequestId.Keyboard, layout, loggerFactory, system, library, nemeio) { }
    }
}
