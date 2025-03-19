namespace Nemeio.Core.Services.Blacklist
{
    public class Blacklist
    {
        public int Id { get; }
        public string Name { get; }
        public bool IsSystem { get; }

        public Blacklist(int id, string name, bool isSystem = false)
        {
            Id = id;
            Name = name;
            IsSystem = isSystem;
        }
    }
}
