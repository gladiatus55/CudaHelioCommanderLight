using CudaHelioCommanderLight.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CudaHelioCommanderLight.Config
{

    // TODO: make as singleton
    public class MetricsConfig
    {

        public enum K0Metrics
        {
            cm2ps,
            m2ps
        }

        public enum VMetrics
        {
            kmps,
            mps
        }

        public enum DtMetrics
        {
            s,
            m
        }

        public enum IntensityMetrics
        {
            npm2ssrGeV
        }

        public K0Metrics K0Metric { get; set; }
        public VMetrics VMetric { get; set; }
        public DtMetrics DtMetric { get; set; }
        public IntensityMetrics IntensityMetric { get; set; }

        public double ErrorFromGev { get; set; }
        public double ErrorToGev { get; set; }
        public bool WasInitialized { get; set; }

        public MetricsConfig()
        {
            K0Metric = K0Metrics.cm2ps;
            VMetric = VMetrics.kmps;
            DtMetric = DtMetrics.s;
            IntensityMetric = IntensityMetrics.npm2ssrGeV;

            // Default Gevs
            ErrorFromGev = 0.5;
            ErrorToGev = 100;

            if (!WasInitialized)
            {
                LoadConfigurationinfo();
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            string kStr, vstr, dtstr, wstr;

            switch (K0Metric)
            {
                case K0Metrics.cm2ps:
                    kStr = "cm^2/s";
                    break;
                case K0Metrics.m2ps:
                    kStr = "m^2/s";
                    break;
                default:
                    kStr = "unknown";
                    break;
            }

            switch (VMetric)
            {
                case VMetrics.kmps:
                    vstr = "km/s";
                    break;
                case VMetrics.mps:
                    vstr = "m/s";
                    break;
                default:
                    vstr = "unknown";
                    break;
            }

            switch (DtMetric)
            {
                case DtMetrics.s:
                    dtstr = "sec";
                    break;
                case DtMetrics.m:
                    dtstr = "min";
                    break;
                default:
                    dtstr = "unknown";
                    break;
            }

            switch (IntensityMetric)
            {
                case IntensityMetrics.npm2ssrGeV:
                    wstr = "n/m^2*s*sr*GeV";
                    break;
                default:
                    wstr = "unknown";
                    break;
            }

            sb.Append(string.Format("Metrics used: K0 ({0}) | dt ({1}) | V ({2}) | w ({3})", kStr, dtstr, vstr, wstr));

            return sb.ToString();
        }

        private static string CACHE_DIR = "cache";
        private readonly string CONFIGURATION_CACHE = Path.Combine(CACHE_DIR, "configInfo.xml");

        public void LoadConfigurationinfo()
        {
            Directory.CreateDirectory(CACHE_DIR);

            if (File.Exists(CONFIGURATION_CACHE))
            {
                try
                {
                    XDocument configInfo = XDocument.Load(CONFIGURATION_CACHE);

                    XElement el = configInfo.Element("configInfo");

                    var errorFromGev = el.Element("errorFromGev").Value;
                    var errorToGev = el.Element("errorToGev").Value;

                    MainHelper.TryConvertToDouble(errorFromGev, out double gevFromError);
                    MainHelper.TryConvertToDouble(errorToGev, out double gevToError);

                    this.ErrorFromGev = gevFromError;
                    this.ErrorToGev = gevToError;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Cannot parse config info xml");
                }
            }
            else
            {
                SaveConfigurationInfo();
            }

            WasInitialized = true;
        }

        public void SaveConfigurationInfo()
        {
            XElement configInfo = new XElement(
                "configInfo",
                    new XElement("errorFromGev", this.ErrorFromGev),
                    new XElement("errorToGev", this.ErrorToGev)
                    );

            configInfo.Save(CONFIGURATION_CACHE);
        }
    }
}
