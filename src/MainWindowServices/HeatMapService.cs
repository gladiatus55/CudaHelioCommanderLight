using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CudaHelioCommanderLight.Models;
using CudaHelioCommanderLight.Operations;

namespace CudaHelioCommanderLight.MainWindowServices;

public class HeatMapService
{
    
    public void DrawHeatmapBtn_Click(object sender, RoutedEventArgs e, ObservableCollection<ExecutionDetail> executionDetailList, int executionDetailSelectedIdx)
    {
        HeatMapGraph heatMap = new HeatMapGraph();
        heatMap.Show();

        ExecutionDetail executionDetail = executionDetailList[executionDetailSelectedIdx];

        int xSize = executionDetail.GetParamK0().Count;
        int ySize = executionDetail.GetParamV().Count;

        if (xSize < 2 || ySize < 2)
        {
            MessageBox.Show("Cannot make map");
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

                if (ex == null)
                {
                    heatPoints[i, j] = new HeatMapGraph.HeatPoint(k0, V, double.NaN);
                    continue;
                }

                double error = ex.ErrorValue;

                heatPoints[i, j] = new HeatMapGraph.HeatPoint(k0, V, error);
            }
        }

        heatMap.SetPoints(heatPoints, xSize, ySize);
        heatMap.Render();
    }
    
    public void DrawAmsHeatmapBtn_Click(string? currentDisplayedAmsInvestigation, List<ErrorStructure> amsComputedErrors, string tag)
    {
        if (amsComputedErrors.Count == 0)
        {
            MessageBox.Show("Empty errors!");
            return;
        }

        DisplayAmsHeatmapWindowOperation.Operate(new DisplayAmsHeatmapModel
        {
            GraphName = currentDisplayedAmsInvestigation,
            Errors = amsComputedErrors,
            Tag = tag
        });
    }
    
}