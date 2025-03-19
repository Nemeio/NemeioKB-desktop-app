using MvvmCross.Platform;
using Nemeio.Presentation;

namespace Nemeio.Wpf.ViewModel
{
    public class TaskBarIconViewModel : BaseViewModel
    {
        public TaskBarIconMenuViewModel Menu { get; }

        public TaskBarIconViewModel()
        {
            Menu = Mvx.Resolve<IMainUserInterface>() as TaskBarIconMenuViewModel;
        }
    }
}
