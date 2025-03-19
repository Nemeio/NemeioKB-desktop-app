using System;
using System.Collections.Generic;
using System.Linq;

namespace Nemeio.Core.DataModels
{
    public class Permutation<T>
    {
        public List<List<T>> Values { get; set; }

        public Permutation()
        {
            Values = new List<List<T>>();
        }

        public Permutation(IEnumerable<List<T>> value)
        {
            Values = value.ToList();
        }

        public void Add(List<T> val) => Values.Add(val);

        public IEnumerable<IEnumerable<T>> Concat(IEnumerable<IEnumerable<T>> val) => Values.Concat(val);

        public Permutation<T> Concat(Permutation<T> val) => new Permutation<T>(Values.Concat(val.Values));

        public void ForEach(Action<IEnumerable<T>> item) => Values.ForEach(item);

        public bool Any() => Values.Any();

        public IEnumerable<IEnumerable<T>> SelectMany(Func<IEnumerable<T>, IEnumerable<T>> collectionSelector,
            Func<IEnumerable<T>, T, IEnumerable<T>> resultSelector)
            => Values.SelectMany(collectionSelector, resultSelector);

        public int Count() => Values.Count();

        public List<T> ElementAt(int index) => Values.ElementAt(index);
    }

    public static class EnumerableExtensions
    {
        public static Permutation<T> ToPermutation<T>(this IEnumerable<List<T>> val) => new Permutation<T>(val);

        public static Permutation<T> ToPermutation<T>(this IEnumerable<IEnumerable<T>> val)
        {
            var result = new List<List<T>>();
            foreach (var value in val)
            {
                result.Add(
                    value.ToList()
                );
            }
            return new Permutation<T>(result);
        }
    }
}
