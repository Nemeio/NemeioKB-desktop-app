using System.Windows;
using Nemeio.Presentation.Menu.Battery;

namespace Nemeio.Wpf.ViewModel
{
    public class BatteryViewModel : BaseViewModel
    {
        private BatterySection _section;

        public string ImagePath
        {
            get
            {
                if (_section == null)
                {
                    return "/Images/Battery-020-48.png";
                }

                switch (_section.Image)
                {
                    case BatteryImageType.Level100:
                        return "/Images/Battery-100-48.png";
                    case BatteryImageType.Level80:
                        return "/Images/Battery-080-48.png";
                    case BatteryImageType.Level60:
                        return "/Images/Battery-060-48.png";
                    case BatteryImageType.Level40:
                        return "/Images/Battery-040-48.png";
                    case BatteryImageType.Level20:
                    default:
                        return "/Images/Battery-020-48.png";
                }
            }
        }
        public string Title => _section?.Text ?? string.Empty;
        public Visibility Visibility
        {
            get
            {
                if (_section == null)
                {
                    return Visibility.Collapsed;
                }

                return _section.Visible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public BatterySection Value
        {
            get => _section;
            set
            {
                if (_section != value)
                {
                    _section = value;

                    NotifyPropertyChanged(nameof(Title));
                    NotifyPropertyChanged(nameof(ImagePath));
                    NotifyPropertyChanged(nameof(Visibility));
                }
            }
        }
    }
}
