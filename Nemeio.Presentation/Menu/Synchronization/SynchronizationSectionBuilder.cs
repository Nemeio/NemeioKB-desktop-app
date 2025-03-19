using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Managers;
using Nemeio.Presentation.Menu.Tools;

namespace Nemeio.Presentation.Menu.Synchronization
{
    public sealed class SynchronizationSectionBuilder : SectionBuilder<SynchronizationSection, SynchronizationProgress>
    {
        public SynchronizationSectionBuilder(ILanguageManager languageManager) 
            : base(languageManager) { }

        public override SynchronizationSection Build(SynchronizationProgress progress)
        {
            if (progress == null)
            {
                return new SynchronizationSection(
                    string.Empty,
                    string.Empty,
                    false
                );
            }
            else
            {
                return new SynchronizationSection(
                    LanguageManager.GetLocalizedValue(StringId.SynchronizationLayoutTitle),
                    $"{progress.Progress} / {progress.Size}",
                    true
                );
            }
        }
    }
}
