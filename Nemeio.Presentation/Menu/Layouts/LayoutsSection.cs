using System;
using System.Collections.Generic;

namespace Nemeio.Presentation.Menu.Layouts
{
    public sealed class LayoutsSection
    {
        public bool Visible { get; private set; }
        public IList<LayoutSubsection> Subsections { get; private set; }

        public LayoutsSection(IList<LayoutSubsection> subsections, bool visible)
        {
            Subsections = subsections ?? throw new ArgumentNullException(nameof(subsections));
            Visible = visible;
        }
    }
}
