using CudaHelioCommanderLight.Models;
using System.IO;
using CudaHelioCommanderLight.Config;
using CudaHelioCommanderLight.Helpers;
using System;

namespace CudaHelioCommanderLight.Operations
{
    public class RenderAmsErrorRatioGraphOperation : Operation<AmsExecutionPltErrorModel>
    {
        private readonly IMainHelper _mainHelper;

        public RenderAmsErrorRatioGraphOperation(IMainHelper mainHelper)
        {
            _mainHelper = mainHelper ?? throw new ArgumentNullException(nameof(mainHelper));
        }

        public static new void Operate(AmsExecutionPltErrorModel amsExecutionErrorModel, IMainHelper mainHelper)
        {
            var amsExecution = amsExecutionErrorModel.AmsExecution;
            var errorStructure = amsExecutionErrorModel.ErrorStructure;


            var metricsConfig = MetricsConfig.GetInstance(mainHelper);
            var plt = amsExecutionErrorModel.Plt;

            if (errorStructure == null)
            {
                return;
            }

            var x = amsExecution.TKin.ToArray();
            var y = amsExecution.Spe1e3.ToArray();
            var x2 = errorStructure.TKinList.ToArray();
            var y2 = errorStructure.Spe1e3binList.ToArray();

            var ratioX = x.Length < x2.Length ? x : x2;
            var ratioY = y.Length < y2.Length ? y : y2;

            var biggerX = x.Length > x2.Length ? x.Length : x2.Length;

            var x1Idx = 0;
            var x2Idx = 0;
            var ratioIdx = 0;

            for (var idx = 0; idx < biggerX; idx++)
            {
                if (ratioIdx >= ratioX.Length)
                {
                    break;
                }

                if ((x.Length > x1Idx && x[x1Idx] == ratioX[ratioIdx]) && (x2.Length > x2Idx && x2[x2Idx] == ratioX[ratioIdx]))
                {
                    ratioY[ratioIdx] = y[x1Idx] / y2[x2Idx];
                    x1Idx++;
                    x2Idx++;
                    ratioIdx++;
                    continue;
                }

                if (x.Length > x1Idx && x[x1Idx] != ratioX[ratioIdx])
                {
                    x1Idx++;
                    continue;
                }

                if (x2.Length > x2Idx && x2[x2Idx] != ratioX[ratioIdx])
                {
                    x2Idx++;
                }
            }

            var amsLegend = Path.GetFileNameWithoutExtension(amsExecution.FileName);
            var errorName = Path.GetFileName(Path.GetDirectoryName(errorStructure.FilePath));
            plt.PlotScatter(ratioX, ratioY, markerSize: 1, color: System.Drawing.Color.Red, label: $"Ratio {amsLegend}/ {errorName}");

            plt.PlotHSpan(
                x1: metricsConfig.ErrorFromGev,
                x2: metricsConfig.ErrorToGev,
                draggable: false,
                color: System.Drawing.Color.FromArgb(0, 255, 0, 0),
                alpha: 0.1
                );

            plt.Title("Ratio graph");
            plt.YLabel("Ratio");
            plt.XLabel("Kinetic Energy [GeV]");
            plt.Legend();
        }
    }
}
