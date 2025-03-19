using Nemeio.UpdateInquiry.Builders;
using Nemeio.UpdateInquiry.Models;

namespace Nemeio.UpdateInquiry.Repositories
{
    public interface IUpdateRepository
    {
        Container GetContainerByEnvironment(UpdateEnvironment environment);
    }
}
