using System;

namespace Nemeio.Core.Keyboard.FactoryReset
{
    public class FactoryResetFailedException : Exception
    {
        public Nemeio Nemeio { get; private set; }

        public FactoryResetFailedException(Nemeio nemeio)
        {
            Nemeio = nemeio ?? throw new ArgumentNullException(nameof(nemeio));
        }
    }
}
