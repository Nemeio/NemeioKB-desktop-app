using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Services.Category
{
    public class Category
    {
        public int Id { get; }
        public int Index { get; }
        public string Title { get; }
        public bool IsDefault { get; }
        public IList<ILayout> Layouts { get; }

        public Category(int index, string title, IEnumerable<ILayout> layouts = null, bool isDefault = false) : this(0, index, title, layouts, isDefault) { }

        public Category(int id, int index, string title, IEnumerable<ILayout> layouts = null, bool isDefault = false)
        {
            Id = id;
            Index = index;
            Title = title;
            IsDefault = isDefault;
            Layouts = layouts != null ? layouts.ToList() : null;
        }
    }
}
