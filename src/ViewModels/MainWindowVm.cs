using CudaHelioCommanderLight.Config;
using CudaHelioCommanderLight.Interfaces;
using System.ComponentModel;

namespace CudaHelioCommanderLight.ViewModels
{
    public  class MainWindowVm : INotifyPropertyChanged, IMetricsConfigObserver
    {
        private string _configuredMetrics = string.Empty;
        public string ConfiguredMetrics
        {
            get { return _configuredMetrics; }
            set
            {
                if (_configuredMetrics != value)
                {
                    _configuredMetrics = value;
                    OnPropertyChanged(nameof(ConfiguredMetrics));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyMetricsConfigChanged(MetricsConfig metricsConfig)
        {
            ConfiguredMetrics = $"Error from {metricsConfig.ErrorFromGev}GeV to {metricsConfig.ErrorToGev}GeV";
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
