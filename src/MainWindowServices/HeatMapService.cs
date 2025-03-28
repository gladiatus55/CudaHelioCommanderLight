using System.Collections.Generic;
using System.Collections.ObjectModel;
using CudaHelioCommanderLight.Models;
using CudaHelioCommanderLight.Operations;
using CudaHelioCommanderLight.Interfaces;
using System.Windows;

namespace CudaHelioCommanderLight.MainWindowServices;

public class HeatMapService
{
    private readonly IDialogService _dialogService;
    private readonly IHeatMapGraphFactory _heatMapGraphFactory;
    private readonly IDisplayAmsHeatmapWindowOperation _displayAmsHeatmapWindowOperation;

    public HeatMapService(IDialogService dialogService,
                        IHeatMapGraphFactory heatMapGraphFactory,
                        IDisplayAmsHeatmapWindowOperation displayAmsHeatmapWindowOperation)
    {
        _dialogService = dialogService;
        _heatMapGraphFactory = heatMapGraphFactory;
        _displayAmsHeatmapWindowOperation = displayAmsHeatmapWindowOperation;
    }

    public void DrawHeatmapBtn(ObservableCollection<ExecutionDetail> executionDetailList, int executionDetailSelectedIdx)
    {
        var heatMap = _heatMapGraphFactory.Create();
        heatMap.Show();

        ExecutionDetail executionDetail = executionDetailList[executionDetailSelectedIdx];

        int xSize = executionDetail.GetParamK0().Count;
        int ySize = executionDetail.GetParamV().Count;

        if (xSize < 2 || ySize < 2)
        {
            _dialogService.ShowMessage("Cannot make map", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        HeatMapGraph.HeatPoint[,] heatPoints = new HeatMapGraph.HeatPoint[xSize, ySize];

        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                double k0 = executionDetail.GetParamK0()[i];
                double V = executionDetail.GetParamV()[j];
                Execution ex = executionDetail.GetExecutionByParam(V, k0);

                heatPoints[i, j] = ex == null
                    ? new HeatMapGraph.HeatPoint(k0, V, double.NaN)
                    : new HeatMapGraph.HeatPoint(k0, V, ex.ErrorValue);
            }
        }

        heatMap.SetPoints(heatPoints, xSize, ySize);
        heatMap.Render();
    }

    public void DrawAmsHeatmapBtn(string? currentDisplayedAmsInvestigation,
                                List<ErrorStructure> amsComputedErrors,
                                string tag)
    {
        if (amsComputedErrors.Count == 0)
        {
            _dialogService.ShowMessage("Empty errors!", "Warning",
                                     MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _displayAmsHeatmapWindowOperation.Operate(new DisplayAmsHeatmapModel
        {
            GraphName = currentDisplayedAmsInvestigation,
            Errors = amsComputedErrors,
            Tag = tag
        });
    }
}
