using CudaHelioCommanderLight.Config;
using ScottPlot;

namespace CudaHelioCommanderLight.Models
{
    public class AmsExecutionPltErrorModel
    {
        public AmsExecution? AmsExecution { get; set; }
        public ErrorStructure? ErrorStructure { get; set; }
        public Plot? Plt { get; set; }
        public MetricsConfig? MetricsConfig { get; set; } // TODO: remove when singleton
    }
}
