using CudaHelioCommanderLight.Models;

namespace CudaHelioCommanderLight.Operations
{
    public class LoadExplorerDataOperation : Operation<WindowExecutionModel, object>
    {
        public static new void Operate(WindowExecutionModel windowExecutionModel)
        {
            var mainWindow = windowExecutionModel.MainWindow;
            var execution = windowExecutionModel.Execution;
            mainWindow.explorerDetailMethodTB.Text = execution.MethodType.ToString();
            mainWindow.explorerDetailK0TB.Text = execution.K0.ToString();
            mainWindow.explorerDetailDtTB.Text = execution.dt.ToString();
            mainWindow.explorerDetailVTB.Text = execution.V.ToString();
            mainWindow.explorerDetailNTB.Text = execution.N.ToString();
        }
    }
}
