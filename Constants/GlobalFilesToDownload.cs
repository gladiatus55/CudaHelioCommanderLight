using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CudaHelioCommanderLight.Constants
{
    public class GlobalFilesToDownload
    {
        public static string RunningInfoFile = "runningInfo.dat";
        public static string GlobalSpe1e3GraphFile = "output_1e3bin_graph_all_spe1e3.png";
        public static string DetailSpe1e3GraphFile = "plotted_graph_spe1e3.png";
        public static string DetailSpe1e3NGraphFile = "plotted_graph_spe1e3N.png";
        public static string DetailSpe1e3FitGraphFile = "plotted_graph_spe1e3_fit.png";
        public static string DetailSpe1e3Fit30GraphFile = "plotted_graph_spe1e3_fit30gev.png";
        public static string DetailOutput1e3binFile = "output_1e3bin.dat";
        public static string DetailOutput1e2binFile = "output_1e2bin.dat";
        public static string DetailOutput4e2binFile = "output_4e2bin.dat";
        public static string DetailOutputlogbinFile = "output_logbin.dat";
        public static string DetailMapaFile = "mapa.dat";
        public static string DetailTimeStatsFile = "timeStats.dat";
        public static string DetailLogFile = "log.dat";

        public static List<string> overallFilesToDownload { get; private set; }

        public static List<string> detailFilesToDownload { get; private set; }

        public static List<string> overviewFilesToDownload { get; private set; }

        public static List<string> detailStatusFilesToDownload { get; private set; }

        public static List<string> alwaysDownloadNew { get; private set; }

        static GlobalFilesToDownload()
        {
            alwaysDownloadNew = new List<string>()
            {
                RunningInfoFile
            };

            overallFilesToDownload = new List<string>()
            {
                GlobalSpe1e3GraphFile,
                "output_1e3bin_graph_all_spe1e3N.png",
                "output_1e3bin_graph_all_spe1e3_fit30gev.png",
                "output_1e3bin_graph_all_spe1e3_fit.png",
                "timeStats.dat",
                RunningInfoFile
            };

            detailFilesToDownload = new List<string>()
            {
                DetailSpe1e3GraphFile,
                "plotted_graph_spe1e3N.png",
                "plotted_graph_spe1e3_fit.png",
                "plotted_graph_spe1e3_fit30gev.png"
            };

            overviewFilesToDownload = new List<string>()
            {
                "output_1e3bin_graph_all_spe1e3.png",
                "runningInfo.dat"
            };

            detailStatusFilesToDownload = new List<string>()
            {
                DetailSpe1e3GraphFile,
                DetailOutput1e3binFile,
            };
        }
    }
}
