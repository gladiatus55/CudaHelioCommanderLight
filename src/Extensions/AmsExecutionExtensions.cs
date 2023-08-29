using CudaHelioCommanderLight.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CudaHelioCommanderLight.Extensions
{
    public static class AmsExecutionExtensions
    {
        public static void AssignLowestValues(this AmsExecution amsExecution, List<ErrorStructure> amsComputedErrors)
        {
            var lowRmsErrorStructure = amsComputedErrors.OrderBy(er => er.Error).FirstOrDefault();
            var minimalMaxErrorStructure = amsComputedErrors.OrderBy(er => er.MaxError).FirstOrDefault();

            var lowRmsErrorStr = lowRmsErrorStructure?.Error.ToString("0.##").Replace(',', '.');
            var minimalMaxErrorStr = minimalMaxErrorStructure?.Error.ToString("0.##").Replace(',', '.');

            amsExecution.LowestRMSError = lowRmsErrorStr;
            amsExecution.LowestRMSErrorFile = lowRmsErrorStructure?.DisplayName;
            amsExecution.MinimalMaxError = minimalMaxErrorStr;
            amsExecution.MinimalMaxErrorFile = minimalMaxErrorStructure?.DisplayName;
        }
    }
}
