using System;
using Nemeio.Core.Managers;

namespace Nemeio.Presentation.Menu.Tools
{
    public abstract class SectionBuilder<T, U>
    {
        protected ILanguageManager LanguageManager { get; private set; }

        public SectionBuilder(ILanguageManager languageManager)
        {
            LanguageManager = languageManager ?? throw new ArgumentNullException(nameof(languageManager));
        }

        public abstract T Build(U obj);
    }
}
