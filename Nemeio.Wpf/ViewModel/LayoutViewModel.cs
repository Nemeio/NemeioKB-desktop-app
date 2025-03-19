using System.Linq;
using System.Windows;
using System.Windows.Input;
using MvvmCross.Platform;
using Nemeio.Core.Layouts.Images.AugmentedLayout;
using Nemeio.Core.Services.Layouts;
using Nemeio.Presentation.Menu.Layouts;

namespace Nemeio.Wpf.ViewModel
{
    public class LayoutViewModel : BaseViewModel
    {
        private LayoutSubsection _subsection;
        private ICommand _toggleAssociation;
        private readonly IAugmentedLayoutImageProvider _augmentedImageProvider;

        public LayoutSubsection Subsection
        {
            get => _subsection;
            set
            {
                _subsection = value;

                NotifyPropertyChanged(nameof(AugmentedHidEnabled));
                NotifyPropertyChanged(nameof(ImagePath));
                NotifyPropertyChanged(nameof(Tag));
                NotifyPropertyChanged(nameof(Title));
                NotifyPropertyChanged(nameof(Selected));
                NotifyPropertyChanged(nameof(Enabled));
                NotifyPropertyChanged(nameof(Layout));
                NotifyPropertyChanged(nameof(ToggleImagePath));
                NotifyPropertyChanged(nameof(ToggleTooltip));
                NotifyPropertyChanged(nameof(Visibility));
            }
        }

        public bool AugmentedHidEnabled => _subsection?.AugmentedHidEnabled ?? false;
        public string ImagePath
        {
            get
            {
                if (_subsection == null)
                {
                    return "/Images/Standard-48.png";
                }

                if (_subsection.Layout.LayoutInfo.Hid)
                {
                    if (AugmentedHidEnabled && _subsection.Layout.LayoutInfo.AugmentedHidEnable && _augmentedImageProvider.AugmentedLayoutImageExists(_subsection.Layout.LayoutInfo.OsLayoutId, _subsection.Layout.LayoutImageInfo.ImageType))
                    {
                        return "/Images/Augmented-48.png";
                    }

                    return "/Images/Standard-48.png";
                }

                return "/Images/Custom-48.png";
            }
        }
        public string Tag => _subsection.Layout.LayoutId ?? string.Empty;
        public string Title => _subsection?.Title ?? string.Empty;
        public bool Selected => _subsection?.IsSelected ?? false;
        public bool Enabled => _subsection?.IsEnabled ?? false;
        public ILayout Layout => _subsection?.Layout;
        public string CleanTitle
        {
            get
            {
                return _subsection.Layout.Title;
            }
        }
        public string CleanSubtitle
        {
            get
            {
                return _subsection.Layout.Subtitle;
            }
        }

        public ICommand ToggleAssociation
        {
            get
            {
                if (_toggleAssociation == null)
                {
                    _toggleAssociation = new RelayCommand(
                        param => this.Apply(),
                        param => this.CanApply()
                    );
                }
                return _toggleAssociation;
            }
        }
        public string ToggleImagePath
        {
            get
            {
                if (_subsection?.Layout?.LayoutInfo?.LinkApplicationEnable ?? false)
                {
                    return "/Images/Enabled-48.png";
                }
                return "/Images/Disabled-48.png";
            }
        }
        public string ToggleTooltip => _subsection?.ToggleTooltip ?? string.Empty;
        public Visibility Visibility
        {
            get
            {
                return _subsection != null && _subsection.Layout != null && _subsection.Layout.LayoutInfo.LinkApplicationPaths.Count() > 0
                    ? Visibility.Visible
                    : Visibility.Hidden;
            }
        }

        public LayoutViewModel()
        {
            _augmentedImageProvider = Mvx.Resolve<IAugmentedLayoutImageProvider>();
        }

        private bool CanApply() => _subsection?.IsEnabled ?? false;

        private void Apply()
        {
            _subsection.Layout.LayoutInfo.LinkApplicationEnable = !_subsection.Layout.LayoutInfo.LinkApplicationEnable;

            NotifyPropertyChanged(nameof(ToggleAssociation));
            NotifyPropertyChanged(nameof(ToggleImagePath));
        }
    }
}
