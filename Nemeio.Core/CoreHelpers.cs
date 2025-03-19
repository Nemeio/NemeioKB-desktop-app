using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nemeio.Core
{
    public static class CoreHelpers
    {
        public static async Task ForEachAsync<T>(this IEnumerable<T> enumerable, Func<T, Task> action)
        {
            foreach (var item in enumerable)
            {
                await action(item);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action(item);
            }
        }

        public static byte[] Append(this byte[] source, byte[] toAppend)
        {
            var buffer = new byte[source.Length + toAppend.Length];
            Array.Copy(source, buffer, source.Length);
            Array.Copy(toAppend, 0, buffer, source.Length, toAppend.Length);
            return buffer;
        }

        static public string TraceBuffer(byte[] buffer, bool withText = false)
        {
            if (buffer == null)
            {
                return $"[Null buffer]";
            }

            if (buffer.Length == 0)
            {
                return $"[Length=0]";
            }

            var strBytes = buffer.Select(l => l.ToString()).Aggregate((l1, l2) => $"{l1}, {l2}");
            var output = $"[Length={buffer.Length}]<{strBytes}>";

            if (withText)
            {
                var str = Encoding.UTF8.GetString(buffer);

                if (str.Any(char.IsLetterOrDigit))
                {
                    return output + $"{str}";
                }
            }

            return output;
        }

        public static string Trace(this byte[] buffer)
        {
            if (buffer.Length > 0)
            {
                return buffer.Select(l => l.ToString()).Aggregate((l1, l2) => $"{l1}, {l2}");
            }

            return string.Empty;
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static string SanitizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }
            return path.ToLower();
        }

        public static IEnumerable<string> SanitizePathList(IEnumerable<string> pathList)
        {
            if (pathList == null)
            {
                return null;
            }
            return pathList
                .Where(path => !string.IsNullOrWhiteSpace(path))
                .Select(path => SanitizePath(path));
        }

        /// <summary>
        /// Generic method to possibly convert an object's instance to an Enum value
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="parameter">Object instance</param>
        /// <returns>Found enumerated value</returns>
        public static T Convert<T>(object parameter)
        {
            return (T) Enum.Parse(typeof(T), parameter.ToString());
        }

        /// <summary>
        /// Utility helper to iterate through an enumeration
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <returns>Enumerable of enum values</returns>
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
