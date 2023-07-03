using CudaHelioCommanderLight.Config;
using CudaHelioCommanderLight.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CudaHelioCommanderLight
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        public MetricsConfig MetricsConfig { get; set; }

        public ConfigWindow(MetricsConfig metricsConfig)
        {
            InitializeComponent();
            this.MetricsConfig = metricsConfig;

            K0UnitsComboBox.ItemsSource = Enum.GetValues(typeof(MetricsConfig.K0Metrics)).Cast<MetricsConfig.K0Metrics>();
            VUnitsComboBox.ItemsSource = Enum.GetValues(typeof(MetricsConfig.VMetrics)).Cast<MetricsConfig.VMetrics>();
            DtUnitsComboBox.ItemsSource = Enum.GetValues(typeof(MetricsConfig.DtMetrics)).Cast<MetricsConfig.DtMetrics>();
            IntensityUnitsComboBox.ItemsSource = Enum.GetValues(typeof(MetricsConfig.IntensityMetrics)).Cast<MetricsConfig.IntensityMetrics>();

            K0UnitsComboBox.SelectedIndex = K0UnitsComboBox.Items.IndexOf(metricsConfig.K0Metric);
            VUnitsComboBox.SelectedIndex = VUnitsComboBox.Items.IndexOf(metricsConfig.VMetric);
            DtUnitsComboBox.SelectedIndex = DtUnitsComboBox.Items.IndexOf(metricsConfig.DtMetric);
            IntensityUnitsComboBox.SelectedIndex = IntensityUnitsComboBox.Items.IndexOf(metricsConfig.IntensityMetric);

            ErrorFromGevTb.Text = metricsConfig.ErrorFromGev.ToString();
            ErrorToGevTb.Text = metricsConfig.ErrorToGev.ToString();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            MetricsConfig.K0Metric = (MetricsConfig.K0Metrics)K0UnitsComboBox.SelectedItem;
            MetricsConfig.VMetric = (MetricsConfig.VMetrics)VUnitsComboBox.SelectedItem;
            MetricsConfig.DtMetric = (MetricsConfig.DtMetrics)DtUnitsComboBox.SelectedItem;
            MetricsConfig.IntensityMetric = (MetricsConfig.IntensityMetrics)IntensityUnitsComboBox.SelectedItem;

            MainHelper.TryConvertToDouble(ErrorFromGevTb.Text, out double gevFromError);
            MainHelper.TryConvertToDouble(ErrorToGevTb.Text, out double gevToError);

            MetricsConfig.ErrorFromGev = gevFromError;
            MetricsConfig.ErrorToGev = gevToError;

            MetricsConfig.SaveConfigurationInfo();

            this.Close();
        }
    }
}
