using CudaHelioCommanderLight.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CudaHelioCommanderLight.Wrappers
{
    public class HeatMapGraphWrapper : IHeatMapGraph
    {
        private readonly HeatMapGraph _heatMapGraph;

        public HeatMapGraphWrapper()
        {
            _heatMapGraph = new HeatMapGraph();
        }

        public void Show() => _heatMapGraph.Show();
        public void SetPoints(HeatMapGraph.HeatPoint[,] heatPoints, int xSize, int ySize)
            => _heatMapGraph.SetPoints(heatPoints, xSize, ySize);
        public void Render() => _heatMapGraph.Render();
        public string GraphTitle
        {
            get => _heatMapGraph.GraphTitle;
            set => _heatMapGraph.GraphTitle = value;
        }

        public string XLabel
        {
            get => _heatMapGraph.XLabel;
            set => _heatMapGraph.XLabel = value;
        }

        public string YLabel
        {
            get => _heatMapGraph.YLabel;
            set => _heatMapGraph.YLabel = value;
        }

        public string ColorbarLabel
        {
            get => _heatMapGraph.ColorbarLabel;
            set => _heatMapGraph.ColorbarLabel = value;
        }
    }

    public class HeatMapGraphFactory : IHeatMapGraphFactory
    {
        public IHeatMapGraph Create() => new HeatMapGraphWrapper();
    }
}
