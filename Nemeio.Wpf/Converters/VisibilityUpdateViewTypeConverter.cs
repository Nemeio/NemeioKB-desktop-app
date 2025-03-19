using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Nemeio.Wpf.ViewModel;

namespace Nemeio.Wpf.Converters
{
    public class VisibilityUpdateViewTypeConverter : IValueConverter
    {
        private const string ActionsState = "Actions";
        private const string DownloadState = "Download";
        private const string InstallingState = "Installing";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var viewType = (UpdateViewType)value;
            var stringParameter = (string)parameter;

            switch(viewType)
            {
                case UpdateViewType.Actions:
                    return stringParameter.Equals(ActionsState) ? Visibility.Visible : Visibility.Hidden;
                case UpdateViewType.Download:
                    return stringParameter.Equals(DownloadState) ? Visibility.Visible : Visibility.Hidden;
                case UpdateViewType.Installing:
                    return stringParameter.Equals(InstallingState) ? Visibility.Visible : Visibility.Hidden;
                default:
                    throw new Exception("Not supported view type");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
