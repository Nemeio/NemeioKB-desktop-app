using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core
{
    public static class LayoutHelper
    {
        public static IList<LayoutId> GetLayoutIds(this IEnumerable<ILayout> layouts)
        {
            return layouts.OrderBy(item => item.LayoutId).Select(item => item.LayoutId).ToList();
        }

        public static string Trace(this IEnumerable<LayoutId> layoutIds)
        {
            if (layoutIds.Count() > 0)
            {
                return layoutIds.Select(l => l.ToString()).Aggregate((l1, l2) => $"{l1}, {l2}");
            }
            return string.Empty;
        }

        public static string Trace(this IEnumerable<LayoutHash> layoutsHashes)
        {
            if (layoutsHashes.Count() > 0)
            {
                return layoutsHashes.Select(l => l.ToString()).Aggregate((l1, l2) => $"{l1}, {l2}");
            }

            return string.Empty;
        }

        public static string Trace(this IEnumerable<ILayout> layout)
        {
            if (layout.Count() > 0)
            {
                return layout.Select(l => l.LayoutId.ToString()).Aggregate((l1, l2) => $"{l1}, {l2}");
            }
            return string.Empty;
        }

        public static bool LayoutListsDiffer(IEnumerable<ILayout> list1, IEnumerable<ILayout> list2)
        {
            if (list1.Count() != list2.Count())
            {
                return false;
            }
            var order1 = list1.GetLayoutIds();
            var order2 = list2.GetLayoutIds();
            return order1.SequenceEqual(order2);
        }
    }
}
