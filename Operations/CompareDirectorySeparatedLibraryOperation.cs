using CudaHelioCommanderLight.Config;
using CudaHelioCommanderLight.Enums;
using CudaHelioCommanderLight.Helpers;
using CudaHelioCommanderLight.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace CudaHelioCommanderLight.Operations
{
    public class CompareDirectorySeparatedLibraryOperation : Operation<CompareLibraryModel, List<ErrorStructure>>
    {
        public static new List<ErrorStructure> Operate(CompareLibraryModel model)
        {
            var amsComputedErrors = new List<ErrorStructure>();

            foreach (var a in Directory.GetDirectories(model.LibPath))
            {
                var libFile = Path.Combine(a, "output_1e3bin.dat");
                var dataExtractSuccess = MainHelper.ExtractOutputDataFile(libFile, out OutputFileContent outputFileContent);

                if (!dataExtractSuccess)
                {
                    MessageBox.Show("Cannot read data values from the input file.");
                    return amsComputedErrors;
                }

                var dividedList = new List<double>();
                for (int idx = 0; idx < outputFileContent.TKinList.Count(); idx++)
                {
                    dividedList.Add(outputFileContent.Spe1e3List[idx] / outputFileContent.Spe1e3NList[idx]);
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
                error.DirName = Path.GetFileName(Path.GetDirectoryName(outputFileContent.FilePath));
                error.TKinList = outputFileContent.TKinList;
                error.Spe1e3binList = outputFileContent.Spe1e3List;
                error.TrySetVAndK0(error.DirName, LibStructureType.DIRECTORY_SEPARATED);

                amsComputedErrors.Add(error);
            }

            return amsComputedErrors;
        }
    }
}
