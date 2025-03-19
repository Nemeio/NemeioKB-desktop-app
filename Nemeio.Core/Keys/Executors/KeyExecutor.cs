using System;
using System.Threading.Tasks;

namespace Nemeio.Core.Keys.Executors
{
    public abstract class KeyExecutor
    {
        public string Data { get; private set; }

        public KeyExecutor(string data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            Data = data;
        }

        public abstract Task ExecuteAsync();
    }
}
