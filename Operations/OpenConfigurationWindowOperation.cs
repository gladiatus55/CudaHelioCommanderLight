using CudaHelioCommanderLight.Config;

namespace CudaHelioCommanderLight.Operations
{
    public static class OpenConfigurationWindowOperation
    {
        public static ConfigWindow Operate(MetricsConfig metricsConfig)
        {
            var configWindow = new ConfigWindow(metricsConfig);
            configWindow.ShowDialog();

            return configWindow;
        }
    }
}
