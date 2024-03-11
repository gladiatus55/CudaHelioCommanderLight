using CudaHelioCommanderLight.Config;

namespace CudaHelioCommanderLight.Models
{
    public class ErrorComputeModel
    {
        public AmsExecution? AmsExecution { get; set; }
        public OutputFileContent? LibraryItem { get; set; }
        public MetricsConfig? MetricsConfig { get; set; } // TODO: Remove when singleton
    }
}
