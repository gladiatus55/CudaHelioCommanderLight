using CudaHelioCommanderLight.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CudaHelioCommanderLight.Commands
{
    public class OpenExplorerCommand // : ICommand
    {
        //public event EventHandler CanExecuteChanged;

        //public bool CanExecute(object parameter)
        //{
        //    return true; // You can add additional conditions here if needed
        //}

        //public void Execute(object parameter)
        //{
        //    System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();

        //    System.Windows.Forms.DialogResult dialogResult = folderDialog.ShowDialog();

        //    if (dialogResult != System.Windows.Forms.DialogResult.OK)
        //    {
        //        return;
        //    }

        //    string selectedFolderPath = folderDialog.SelectedPath;

        //    if (string.IsNullOrEmpty(selectedFolderPath))
        //    {
        //        return;
        //    }

        //    // Access your view model and perform the necessary logic
        //    MainViewModel viewModel = parameter as MainViewModel;
        //    if (viewModel != null)
        //    {
        //        viewModel.SwitchPanels(PanelType.STATUS_CHECKER);
        //        ExecutionStatus executionStatus = MainHelper.ExtractOfflineExecStatus(selectedFolderPath);
        //        viewModel.ExecutionDetailList = new ObservableCollection<ExecutionDetail>(executionStatus.activeExecutions);
        //        // Make sure to implement the necessary properties in the view model
        //    }
        //}
    }
}
