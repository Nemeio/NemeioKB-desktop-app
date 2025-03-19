using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Services
{
    public interface ILayoutValidityChecker
    {
        bool Check(ILayout layout);
    }
}
