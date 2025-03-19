using System.Collections.Generic;

namespace Nemeio.Core.Services.Blacklist
{
    public interface IBlacklistDbRepository
    {
        Blacklist FindBlacklistByName(string name);

        Blacklist FindBlacklistById(int id);

        IEnumerable<Blacklist> ReadSystemBlacklists();

        IEnumerable<Blacklist> ReadBlacklists();

        Blacklist SaveBlacklist(string name);

        bool DeleteBlacklist(int id);
    }
}
