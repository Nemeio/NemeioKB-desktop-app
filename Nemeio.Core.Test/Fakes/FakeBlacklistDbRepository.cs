using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.Services.Blacklist;

namespace Nemeio.Core.Test.Fakes
{
    public class FakeBlacklistDbRepository : IBlacklistDbRepository
    {
        public const string NemeioString = "nemeio";
        public const string ExplorerString = "explorer";

        private int lastAddedIndex = 5;
        public bool UpdateCalled = false;
        public bool FindOneByIdCalled = false;
        public bool DeleteCalled = false;
        public List<Blacklist> Blacklists { get; } = new List<Blacklist>()
        {
            new Blacklist(1, ExplorerString, true),
            new Blacklist(2, NemeioString, true),
            new Blacklist(3, "third"),
            new Blacklist(4, "fourth"),
            new Blacklist(5, "fifth")
        };

        public Blacklist FindBlacklistByName(string name)
        {
            return Blacklists.FirstOrDefault(item => item.Name.Equals(name));
        }

        public Blacklist FindBlacklistById(int id)
        {
            return Blacklists.FirstOrDefault(item => item.Id == id);
        }

        public IEnumerable<Blacklist> ReadSystemBlacklists()
        {
            return Blacklists.Where(item => item.IsSystem == true);
        }

        public IEnumerable<Blacklist> ReadBlacklists()
        {
            return Blacklists.Where(item => item.IsSystem == false);
        }

        public Blacklist SaveBlacklist(string name)
        {
            var result = FindBlacklistByName(name);
            if (result == null)
            {
                lastAddedIndex += 1;
                Blacklists.Add(new Blacklist(lastAddedIndex, name, false));
                result = FindBlacklistByName(name);
            }
            return result;
        }

        public bool DeleteBlacklist(int id)
        {
            var item = FindBlacklistById(id);
            if (item == null || item.IsSystem)
            {
                return false;
            }
            Blacklists.Remove(item);
            return true;
        }
    }
}
