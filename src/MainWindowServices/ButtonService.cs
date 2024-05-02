using System.Collections.ObjectModel;
using System.Windows;
using CudaHelioCommanderLight.Models;
using CudaHelioCommanderLight.Operations;
using Microsoft.Win32;

namespace CudaHelioCommanderLight.MainWindowServices;

public class ButtonService
{
    public void AboutUsButton()
    {
        string message = "Slovak Academy of Sciences\n\nDeveloped by: Martin Nguyen, Pavol Bobik\n\nCopyright 2023";
        MessageBox.Show(message, "About Us", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    
    public void ExportJsonBtn(ObservableCollection<ExecutionDetail> executionDetailList, int executionDetailSelectedIdx)
    {
        ExecutionDetail executionDetail = executionDetailList[executionDetailSelectedIdx];

        if (executionDetail == null)
        {
            return;
        }

        SaveFileDialog fileDialog = new SaveFileDialog();

        fileDialog.Filter = "JSON File|*.json";
        fileDialog.Title = "Save JSON File";
        fileDialog.ShowDialog();

        // If the file name is not an empty string open it for saving.
        if (fileDialog.FileName != "")
        {
            var exportModel = new ExecutionListExportModel
            {
                Executions = executionDetail.Executions,
                FilePath = fileDialog.FileName
            };
            ExportAsJsonOperation.Operate(exportModel);
        }
    }
    
    
}