using Nemeio.Core.DataModels;

namespace Nemeio.Core.Services.Layouts
{
    public class OsLayoutId : StringType<OsLayoutId>
    {
        public OsLayoutId(string id)
            : base(id) { }

        public OsLayoutId(string id, string name)
            : base(id)
        {
            Name = name;
        }

        public string Id => Value;
        public string Name { get; set; }
        public int Order { get; set; }

        public static OsLayoutId Empty => new OsLayoutId(string.Empty);
    }
}
