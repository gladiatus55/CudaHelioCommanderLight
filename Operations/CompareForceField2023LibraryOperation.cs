using CudaHelioCommanderLight.Enums;
using CudaHelioCommanderLight.Helpers;
using CudaHelioCommanderLight.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace CudaHelioCommanderLight.Operations
{
    public class CompareForceField2023LibraryOperation : Operation<CompareLibraryModel, List<ErrorStructure>>
    {
        // TODO: Refactor
        public static new List<ErrorStructure> Operate(CompareLibraryModel model)
        {
            var forceFieldErrors = new List<ErrorStructure>();

            foreach (var libFile in Directory.GetFiles(model.LibPath))
            {
                var dataExtractSuccess = MainHelper.ExtractForceFieldOutputDataFile(libFile, out OutputFileContent outputFileContent);

                if (!dataExtractSuccess)
                {
                    MessageBox.Show("Cannot read data values from the input file.");
                    return forceFieldErrors;
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
                error.DisplayName = Path.GetFileName(outputFileContent.FilePath);
                error.TKinList = outputFileContent.TKinList;
                error.Spe1e3binList = outputFileContent.Spe1e3List;

                // tmp - FILES_FORCEFIELD2023_COMPARISION
                var fileName = Path.GetFileNameWithoutExtension(error.FilePath);
                var success = int.TryParse(fileName, out int xval);
                if (success)
                {
                    error.V = xval;
                }
                error.K0 = 0;
                // end tmp

                forceFieldErrors.Add(error);
            }

            return forceFieldErrors;
        }
    }
}
