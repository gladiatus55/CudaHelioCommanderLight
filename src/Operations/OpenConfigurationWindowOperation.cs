using CudaHelioCommanderLight.Config;
using CudaHelioCommanderLight.Helpers;

namespace CudaHelioCommanderLight.Operations
{
    public class OpenConfigurationWindowOperation : Operation<MetricsConfig, ConfigWindow>
    {
        public static new ConfigWindow Operate(MetricsConfig metricsConfig, IMainHelper mainHelper)
        {
            var configWindow = new ConfigWindow(metricsConfig, mainHelper);
            configWindow.ShowDialog();

            return configWindow;
        }
    }
}
