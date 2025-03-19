namespace Nemeio.Wpf.ViewModel
{
    public class CloseWindowViewModel : BaseViewModel
    {
        public string Title
        {
            get
            {
                return _languageManager.GetLocalizedValue(Core.DataModels.Locale.StringId.CommonButtonQuit);
            }
        }

        public string Question
        {
            get
            {
                return _languageManager.GetLocalizedValue(Core.DataModels.Locale.StringId.QuitQuestion);
            }
        }
        public string Yes
        {
            get
            {
                return _languageManager.GetLocalizedValue(Core.DataModels.Locale.StringId.CommonYes).ToUpperInvariant();
            }
        }

        public string No
        {
            get
            {
                return _languageManager.GetLocalizedValue(Core.DataModels.Locale.StringId.CommonNo).ToUpperInvariant();
            }
        }

        public string Disclaimer
        {
            get
            {
                return _languageManager.GetLocalizedValue(Core.DataModels.Locale.StringId.QuitDisclaimer);
            }
        }
    }
}
