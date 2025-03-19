using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MvvmCross.Platform;
using Nemeio.Core.Managers;
using Windows.UI.Xaml;

namespace Nemeio.Wpf.ViewModel
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        protected readonly ILanguageManager _languageManager;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler OnRequestClose;

        public BaseViewModel()
        {
            _languageManager = Mvx.Resolve<ILanguageManager>();
            _languageManager.LanguageChanged += LanguageManager_LanguageChanged;
        }

        public virtual void LanguageChanged()
        {
            //  Override if needed
        }

        private void LanguageManager_LanguageChanged(object sender, System.EventArgs e)
        {
            LanguageChanged();
        }

        protected void RequestClose() => OnRequestClose?.Invoke(this, EventArgs.Empty);

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Template for property change event notification using this as a base template to prevent property misspelling
        /// (property identified from variable name)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="aVariable">Variable to be changed</param>
        /// <param name="aValue">Target to be set to variable</param>
        /// <param name="aPropertyName"></param>
        /// <returns></returns>
        protected bool NotifyPropertyChanged<T>(ref T aVariable, T aValue, [CallerMemberName] string aPropertyName = null)
        {
            if (object.Equals(aVariable, aValue))
            {
                return false;
            }

            aVariable = aValue;
            NotifyPropertyChanged(aPropertyName);
            return true;
        }
    }
}
