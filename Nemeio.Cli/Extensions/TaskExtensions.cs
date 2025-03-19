using System;
using System.Threading.Tasks;

namespace Nemeio.Cli.Extensions
{
    internal static class TaskExtensions
    {
        public static Task TimeoutAfter(this Task task, TimeSpan timeSpan) => TimeoutAfter(task, (int)timeSpan.TotalMilliseconds);

        public static async Task TimeoutAfter(this Task task, int millisecondsTimeout)
        {
            if (task == await Task.WhenAny(task, Task.Delay(millisecondsTimeout)))
                await task;
            else
                throw new TimeoutException();
        }
    }
}
