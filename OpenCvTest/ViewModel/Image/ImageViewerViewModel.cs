using System.Windows.Media.Imaging;
using AutomatedRodentTracker.Commands;

namespace AutomatedRodentTracker.ViewModel.Image
{
    public class ImageViewerViewModel : WindowViewModelBase
    {
        private ActionCommand m_OkCommand;
        private BitmapSource m_Image;

        public ActionCommand OkCommand
        {
            get
            {
                return m_OkCommand ?? (m_OkCommand = new ActionCommand
                {
                    ExecuteAction = Ok,
                });
            }
        }

        public BitmapSource Image
        {
            get
            {
                return m_Image;
            }
            set
            {
                if (ReferenceEquals(m_Image, value))
                {
                    return;
                }

                m_Image = value;

                NotifyPropertyChanged();
            }
        }

        private void Ok()
        {
            ExitResult = WindowExitResult.Ok;
            CloseWindow();
        }
    }
}
