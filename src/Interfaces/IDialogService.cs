using System.Windows;

namespace CudaHelioCommanderLight.Interfaces
{
    public interface IDialogService
    {
        bool SaveFileDialog(out string filePath, string filter);
        void ShowMessage(string text, string caption, MessageBoxButton button, MessageBoxImage icon);
    }
}

