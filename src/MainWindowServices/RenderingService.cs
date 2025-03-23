using CudaHelioCommanderLight.Config;
using CudaHelioCommanderLight.Interfaces;
using CudaHelioCommanderLight.Models;
using CudaHelioCommanderLight.Operations;
using Microsoft.Win32;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using CudaHelioCommanderLight.Helpers;

namespace CudaHelioCommanderLight.MainWindowServices
{
    public class RenderingService
    {
        private readonly IMainHelper _mainHelper;
        public RenderingService(IMainHelper mainHelper)
        {
            _mainHelper = mainHelper;
        }
        public ErrorStructure? AmsErrorsListBox_SelectionChanged(ErrorStructure errorStructure, WpfPlot amsGraphWpfPlot,
            WpfPlot amsGraphRatioWpfPlot, AmsExecution amsExecution)
        {
            ErrorStructure error = errorStructure;
            if (error == null)
            {
                return null;
            }

            AmsExecution exD = amsExecution;

            RenderAmsGraph(exD, amsGraphWpfPlot, error);
            RenderAmsRatioGraph(exD, amsGraphRatioWpfPlot, error);

            return error;
        }

        public void RenderAmsGraph(AmsExecution amsExecution, WpfPlot amsGraphWpfPlot,
            ErrorStructure? errorStructure = null)
        {
            amsGraphWpfPlot.Reset();
            var amsExecutionErrorModel = new AmsExecutionPltErrorModel()
            {
                AmsExecution = amsExecution,
                ErrorStructure = errorStructure,
                Plt = amsGraphWpfPlot.plt,
            };

            RenderAmsErrorGraphOperation.Operate(amsExecutionErrorModel, _mainHelper);
            amsGraphWpfPlot.Render();
        }

        private void RenderAmsRatioGraph(AmsExecution amsExecution, WpfPlot amsGraphRatioWpfPlot,
            ErrorStructure? errorStructure = null)
        {
            amsGraphRatioWpfPlot.Reset();
            var amsExecutionErrorModel = new AmsExecutionPltErrorModel()
            {
                AmsExecution = amsExecution,
                ErrorStructure = errorStructure,
                Plt = amsGraphRatioWpfPlot.plt,
            };

            RenderAmsErrorRatioGraphOperation.Operate(amsExecutionErrorModel,_mainHelper);
            amsGraphRatioWpfPlot.Render();
        }

        public void CreateErrorGraph(DataGrid activeCalculationsDataGrid)
        {

            OpenFileDialog fileDialog = new OpenFileDialog();
            List<ExecutionDetail> selectedExecutionDetails = new List<ExecutionDetail>();


            if (fileDialog.ShowDialog() == false)
            {
                return;
            }

            string filePath = fileDialog.FileName;

            if (filePath == null)
            {
                return;
            }

            bool dataExtractSuccess = _mainHelper.ExtractOutputDataFile(filePath, out OutputFileContent outputFileContent);

            if (!dataExtractSuccess)
            {
                MessageBox.Show("Cannot read data values from the input file.");
                return;
            }
            foreach (ExecutionDetail executionDetail in (ObservableCollection<ExecutionDetail>)activeCalculationsDataGrid.ItemsSource)
            {
                if (executionDetail.IsSelected)
                {
                    selectedExecutionDetails.Add(executionDetail);

                    foreach (Execution execution in executionDetail.Executions)
                    {
                        ExecutionHelper.InitializeOutput1e3BinDataFromOnlineDir(execution, _mainHelper);
                        if (execution.StandardDeviatons != null)
                        {
                            try { 
                                execution.ComputeError(outputFileContent, MetricsConfig.GetInstance(_mainHelper));
                            }
                            catch (ArgumentOutOfRangeException ex)
                            {
                                MessageBox.Show(ex.ToString());
                                return;
                            }
                        }
                    }
                }
            }

            RenderGraphOfErrors(selectedExecutionDetails);
        }

        private void RenderGraphOfErrors(List<ExecutionDetail> selectedExecutionDetails)
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
}
