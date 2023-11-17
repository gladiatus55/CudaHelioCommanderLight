using CudaHelioCommanderLight.Config;

namespace CudaHelioCommanderLight.Interfaces
{
    public interface IMetricsConfigObserver
    {
        void NotifyMetricsConfigChanged(MetricsConfig metricsConfig);
    }
}
