using System.Windows;

namespace CudaHelioCommanderLight.MainWindowServices;

public class ButtonService
{
    public void AboutUsButton_Click(object sender, RoutedEventArgs e)
    {
        string message = "Slovak Academy of Sciences\n\nDeveloped by: Martin Nguyen, Pavol Bobik\n\nCopyright 2023";
        MessageBox.Show(message, "About Us", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}