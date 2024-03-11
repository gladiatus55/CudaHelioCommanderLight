using CudaHelioCommanderLight.Config;

namespace CudaHelioCommanderLight.Models
{
    public class CompareLibraryModel
    {
        public string? LibPath { get; set; }
        public AmsExecution? AmsExecution { get; set; }
        public MetricsConfig? MetricsConfig { get; set; } // TODO: Remove when singleton
    }
}
