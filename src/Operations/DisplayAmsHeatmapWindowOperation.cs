using CudaHelioCommanderLight.Interfaces;
using CudaHelioCommanderLight.Models;
using System.Linq;
using System.Windows;

namespace CudaHelioCommanderLight.Operations
{
    public class DisplayAmsHeatmapWindowOperation : IDisplayAmsHeatmapWindowOperation
    {
        private readonly IDialogService _dialogService;
        private readonly IHeatMapGraphFactory _heatMapGraphFactory;

        public DisplayAmsHeatmapWindowOperation(IDialogService dialogService,
                                              IHeatMapGraphFactory heatMapGraphFactory)
        {
            _dialogService = dialogService;
            _heatMapGraphFactory = heatMapGraphFactory;
        }

        public void Operate(DisplayAmsHeatmapModel model)
        {
            var amsComputedErrors = model.Errors;
            var heatMap = _heatMapGraphFactory.Create();
            heatMap.Show();

            var sortedErrors = amsComputedErrors.OrderBy(err => err.K0)
                                              .ThenBy(err => err.V)
                                              .ToList();

            var k0List = amsComputedErrors.Select(err => err.K0).Distinct().OrderBy(k => k);
            var vList = amsComputedErrors.Select(err => err.V).Distinct().OrderBy(v => v);
            int xSize = k0List.Count();
            int ySize = vList.Count();

            if (xSize < 2 || ySize < 2)
            {
                _dialogService.ShowMessage("Cannot make map, size too small", "Error",
                                         MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            HeatMapGraph.HeatPoint[,] heatPoints = new HeatMapGraph.HeatPoint[xSize, ySize];

            for (int i = 0; i < xSize; i++)
            {
                var currK0 = k0List.ElementAt(i);
                for (int j = 0; j < ySize; j++)
                {
                    var currV = vList.ElementAt(j);
                    var computedError = sortedErrors.FirstOrDefault(err =>
                        err.K0 == currK0 && err.V == currV);

                    heatPoints[i, j] = model.Tag switch
                    {
                        "RMS" => new HeatMapGraph.HeatPoint(currK0, currV, computedError?.Error ?? double.NaN),
                        "LowMaxError" => new HeatMapGraph.HeatPoint(currK0, currV, computedError?.MaxError ?? double.NaN),
                        _ => new HeatMapGraph.HeatPoint(currK0, currV, double.NaN)
                    };
                }
            }

            heatMap.SetPoints(heatPoints, xSize, ySize);
            heatMap.GraphTitle = model.GraphName ?? "";
            heatMap.XLabel = "V [km/s]";
            heatMap.YLabel = "K0 [cm^2/s]";
            heatMap.ColorbarLabel = "Deviation [%]";
            heatMap.Render();
        }
    }

    public interface IDisplayAmsHeatmapWindowOperation
    {
        void Operate(DisplayAmsHeatmapModel model);
    }
}
