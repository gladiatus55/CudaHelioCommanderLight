using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CudaHelioCommanderLight.Enums;
using CudaHelioCommanderLight.Helpers;
using CudaHelioCommanderLight.Models;

namespace CudaHelioCommanderLight.Operations;

public class CompareLibraryOperation : Operation<CompareLibraryModel, List<ErrorStructure>>
{
    public static List<ErrorStructure> Operate(CompareLibraryModel model, LibStructureType libStructureType)
    {
        var errors = new List<ErrorStructure>();

        var isFileDirectory = IsDirectory(libStructureType);
        
        var files = isFileDirectory
            ? Directory.GetDirectories(model.LibPath)
            : Directory.GetFiles(model.LibPath);

        foreach (var file in files)
        {
            var libFile = isFileDirectory
                ? Path.Combine(file, "output_1e3bin.dat")
                : file;
            
            var dataExtractSuccess = MainHelper.ExtractOutputDataFile(libFile, out OutputFileContent outputFileContent);

            if (!dataExtractSuccess)
            {
                MessageBox.Show("Cannot read data values from the input file.");
                return errors;
            }

            var dividedList = new List<double>();
            for (int idx = 0; idx < outputFileContent.TKinList.Count(); idx++)
            {
                dividedList.Add(isFileDirectory ? outputFileContent.Spe1e3List[idx] / outputFileContent.Spe1e3NList[idx] : outputFileContent.Spe1e3List[idx]);
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
            error.DisplayName = GetDisplayName(outputFileContent.FilePath, isFileDirectory);
            error.TKinList = outputFileContent.TKinList;
            error.Spe1e3binList = outputFileContent.Spe1e3List;

            switch (libStructureType)
            {
                case LibStructureType.DIRECTORY_SEPARATED:
                    error.TrySetVAndK0(error.DisplayName, libStructureType);
                    break;
                case LibStructureType.FILES_SOLARPROP_LIB:
                    error.TrySetVAndK0(Path.GetFileName(outputFileContent.FilePath), libStructureType);
                    break;
                default:
                {
                    var fileName = Path.GetFileNameWithoutExtension(error.FilePath);
                    var success = int.TryParse(fileName, out int xval);
                    if (success)
                    {
                        error.V = xval;
                    }
                    error.K0 = 0;
                    break;
                }
            }
            errors.Add(error);
        }
        return errors;
    }
    
    private static string? GetDisplayName(string outputFilePath, bool isFileDirectory)
    {
        return isFileDirectory ? Path.GetFileName(Path.GetDirectoryName(outputFilePath)) : Path.GetFileName(outputFilePath);
    }
    
    private static bool IsDirectory(LibStructureType libStructureType)
    {
        return libStructureType is LibStructureType.DIRECTORY_SEPARATED;
    }
}