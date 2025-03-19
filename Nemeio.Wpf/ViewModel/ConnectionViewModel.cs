using System.Windows;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Presentation.Menu.Connection;

namespace Nemeio.Wpf.ViewModel
{
    public class ConnectionViewModel : BaseViewModel
    {
        private ConnectionSection _section;
        
        public string ImagePath
        {
            get
            {
                if (_section == null)
                {
                    return "/Images/Usb-48.png";
                }

                switch (_section.Communication)
                {
                    case CommunicationType.BluetoothLE:
                        return "/Images/Bluetooth-48.png";
                    default:
                    case CommunicationType.Serial:
                        return "/Images/Usb-48.png";
                }
            }
        }
        public string Title => _section?.Title ?? string.Empty;
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

        public ConnectionSection Section 
        {
            get => _section;
            set
            {
                _section = value;
                NotifyPropertyChanged(nameof(Visibility));
                NotifyPropertyChanged(nameof(Title));
                NotifyPropertyChanged(nameof(ImagePath));
            }
        }

        public ConnectionViewModel() { }
    }
}
