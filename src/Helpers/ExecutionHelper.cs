﻿using CudaHelioCommanderLight.Constants;
using CudaHelioCommanderLight.Models;
using System.IO;

namespace CudaHelioCommanderLight.Helpers
{
    public static class ExecutionHelper
    {
        public static void InitializeOutput1e3BinDataFromOnlineDir(Execution execution)
        {
            string local1e3binFilePath = Path.Combine(execution.LocalDirPath, GlobalFilesToDownload.DetailOutput1e3binFile);

            if (File.Exists(local1e3binFilePath))
            {
                MainHelper.ExtractOutputDataFile(local1e3binFilePath, out OutputFileContent outputFileContent);

                execution.Spe1e3 = outputFileContent.Spe1e3List;
                execution.Spe1e3N = outputFileContent.Spe1e3NList;
                execution.TKin = outputFileContent.TKinList;
                execution.StandardDeviatons = outputFileContent.StdDevList;
            }
        }
    }
}
