using System;

namespace Nemeio.Tools.Testing.Update.Core.Update
{
    public class UpdateVersionRange
    {
        public Version Start { get; private set; }

        public Version End { get; private set; }

        public UpdateVersionRange(string start, string end)
            : this(new Version(start), new Version(end)) { }

        public UpdateVersionRange(Version start, Version end)
        {
            Start = start ?? throw new ArgumentNullException(nameof(start));
            End = end ?? throw new ArgumentNullException(nameof(end));

            if (End <= Start)
            {
                throw new ArgumentException("End version must be greater than start version");
            }
        }
    }
}
