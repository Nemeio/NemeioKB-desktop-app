using System.Collections.Generic;

namespace Nemeio.Keyboard.Communication.Tools.Utils
{
    public static class Extensions
    {
        public static List<T> AddChainable<T>(this List<T> list, T element)
        {
            list.Add(element);
            return list;
        }

        public static List<T> AddChainable<T>(this List<T> list, IEnumerable<T> elements)
        {
            list.AddRange(elements);
            return list;
        }
    }
}
