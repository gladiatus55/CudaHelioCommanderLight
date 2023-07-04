using CudaHelioCommanderLight.Enums;
using CudaHelioCommanderLight.Helpers;
using CudaHelioCommanderLight.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CudaHelioCommanderLight.Operations
{
    internal class CompareGeliosphereLibraryOperation : Operation<CompareLibraryModel, List<ErrorStructure>>
    {
        public static new List<ErrorStructure> Operate(CompareLibraryModel model)
        {
            var amsComputedErrors = new List<ErrorStructure>();

            foreach (var libFile in Directory.GetFiles(model.LibPath))
            {
                var dataExtractSuccess = MainHelper.ExtractOutputDataFile(libFile, out OutputFileContent outputFileContent);

                if (!dataExtractSuccess)
                {
                    MessageBox.Show("Cannot read data values from the input file.");
                    return amsComputedErrors;
                }

                var dividedList = new List<double>();
                for (int idx = 0; idx < outputFileContent.TKinList.Count(); idx++)
                {
                    dividedList.Add(outputFileContent.Spe1e3List[idx]);
                }
                outputFileContent.Spe1e3List = dividedList;

                var error = new ErrorStructure();
                var computedError = ComputeErrorOperation.Operate(new ErrorComputeModel()
                {
                    AmsExecution = model.AmsExecution,
                    LibraryItem = outputFileContent,
                    MetricsConfig = model.MetricsConfig,
                });
                error.Error = computedError.Error;
                error.MaxError = computedError.MaxError;
                error.FilePath = outputFileContent.FilePath;
                error.DirName = Path.GetFileName(outputFileContent.FilePath);
                error.TKinList = outputFileContent.TKinList;
                error.Spe1e3binList = outputFileContent.Spe1e3List;
                error.TrySetVAndK0(Path.GetFileName(outputFileContent.FilePath), LibStructureType.FILES_SOLARPROP_LIB);

                amsComputedErrors.Add(error);
            }

            return amsComputedErrors;
        }
    }
}
