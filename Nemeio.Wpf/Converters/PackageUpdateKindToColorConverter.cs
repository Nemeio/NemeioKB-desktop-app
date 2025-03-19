using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Nemeio.Core.PackageUpdater;

namespace Nemeio.Wpf.Converters
{
    public class PackageUpdateKindToColorConverter : IValueConverter
    {
        private const string GreenColorName = "LdlcGreen";
        private const string YellowColorName = "LdlcMustard";
        private const string RedColorName = "LdlcRed";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var kind = (PackageUpdateState)value;
            switch (kind)
            {
                case PackageUpdateState.UpdateChecking:
                case PackageUpdateState.Idle:
                case PackageUpdateState.UpdateSucceed:
                    return GetColorFromName(GreenColorName);
                case PackageUpdateState.DownloadPending:
                case PackageUpdateState.ApplyUpdate:
                    return GetColorFromName(YellowColorName);
                case PackageUpdateState.UpdatePending:
                case PackageUpdateState.UpdateFailed:
                    return GetColorFromName(RedColorName);
                default:
                    throw new InvalidOperationException("Invalid kind of package");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private SolidColorBrush GetColorFromName(string name)
        {
            var appResource = Application.Current.Resources[name];
            if (appResource != null)
            {
                return (SolidColorBrush)appResource;
            }

            foreach (var dictionary in Application.Current.Resources.MergedDictionaries)
            {
                var color = dictionary[name];
                if (color != null)
                {
                    return (SolidColorBrush)color;
                }
            }

            return null;
        }
    }
}
