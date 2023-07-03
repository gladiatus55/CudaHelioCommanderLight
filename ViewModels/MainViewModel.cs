using System.Windows.Input;
using System.Windows;
using CudaHelioCommanderLight.Commands;

namespace CudaHelioCommanderLight.ViewModels
{
    public class MainViewModel
    {
        public ICommand ButtonClickCommand { get; }

        public MainViewModel()
        {
            ButtonClickCommand = new RelayCommand(ButtonClickHandler);
        }

        private void ButtonClickHandler(object parameter)
        {
            // Handle the button click event here
            // Add your clean code logic
            // For example, you can display a message box:
            MessageBox.Show("Button clicked!");
        }
    }
}
