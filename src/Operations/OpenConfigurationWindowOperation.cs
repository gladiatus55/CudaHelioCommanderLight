using CudaHelioCommanderLight.Config;

namespace CudaHelioCommanderLight.Operations
{
    public class OpenConfigurationWindowOperation : Operation<MetricsConfig, ConfigWindow>
    {
        public static new ConfigWindow Operate(MetricsConfig metricsConfig)
        {
            var configWindow = new ConfigWindow(metricsConfig);
            configWindow.ShowDialog();

            return configWindow;
        }
    }
}
