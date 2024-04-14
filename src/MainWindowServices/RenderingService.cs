using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CudaHelioCommanderLight.Exceptions;
using CudaHelioCommanderLight.Helpers;
using CudaHelioCommanderLight.Models;
using CudaHelioCommanderLight.Operations;
using Microsoft.Win32;
using ScottPlot;

namespace CudaHelioCommanderLight.MainWindowServices;

public class RenderingService
{
    public void RenderAmsGraph(AmsExecution amsExecution, WpfPlot amsGraphWpfPlot, ErrorStructure? errorStructure = null)
    {
        amsGraphWpfPlot.Reset();
        var amsExecutionErrorModel = new AmsExecutionPltErrorModel()
        {
            AmsExecution = amsExecution,
            ErrorStructure = errorStructure,
            Plt = amsGraphWpfPlot.plt,
        };

        RenderAmsErrorGraphOperation.Operate(amsExecutionErrorModel);
        amsGraphWpfPlot.Render();
    }
    
    public void RenderAmsRatioGraph(AmsExecution amsExecution, WpfPlot amsGraphRatioWpfPlot,  ErrorStructure? errorStructure = null)
    {
        amsGraphRatioWpfPlot.Reset();
        var amsExecutionErrorModel = new AmsExecutionPltErrorModel()
        {
            AmsExecution = amsExecution,
            ErrorStructure = errorStructure,
            Plt = amsGraphRatioWpfPlot.plt,
        };

        RenderAmsErrorRatioGraphOperation.Operate(amsExecutionErrorModel);
        amsGraphRatioWpfPlot.Render();
    }
    
    
    public void RenderGraphOfErrors(List<ExecutionDetail> selectedExecutionDetails)
    {
        List<System.Drawing.Color> colorList = new List<System.Drawing.Color>()
        {
            System.Drawing.Color.Red,
            System.Drawing.Color.Green,
            System.Drawing.Color.Blue,
            System.Drawing.Color.Aqua,
            System.Drawing.Color.Orange
        };
        var plt = new ScottPlot.Plot(600, 400);
            
        List<string> vAndKToIdx = new List<string>();

        //foreach (GraphInfo graphInfo in loadedGraphs)
        for (int idx = 0; idx < selectedExecutionDetails.Count; idx++)
        {
            List<Execution> nLowestExecutions = selectedExecutionDetails[idx].GetLowestExecutions(10);

            foreach (Execution e in nLowestExecutions)
            {
                string stringify = "V=" + e.V + "K0=" + e.K0;

                if (!vAndKToIdx.Contains(stringify))
                {
                    vAndKToIdx.Add(stringify);
                }
            }

            double[] x = nLowestExecutions.Select(e => (double)vAndKToIdx.IndexOf("V=" + e.V + "K0=" + e.K0)).ToArray();
            double[] y = nLowestExecutions.Select(e => e.ErrorValue).ToArray();

            plt.PlotScatter(x, y, markerSize: 5, lineWidth: 0, color: colorList[idx], label: selectedExecutionDetails[idx].FolderName);
        }

        plt.XTicks(Enumerable.Range(0, vAndKToIdx.Count).Select(a => (double)a).ToArray(), vAndKToIdx.ToArray());

        plt.Title("Plotted errors");
        plt.YLabel("Error");
        plt.XLabel("Index");
        plt.Legend();
        var plotViewer = new ScottPlot.WpfPlotViewer(plt);

        plotViewer.Show();
    }
    
}