using CudaHelioCommanderLight.Dtos;
using CudaHelioCommanderLight.Models;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using CudaHelioCommanderLight.Interfaces;

namespace CudaHelioCommanderLight.Operations
{
    public class ExportAsJsonOperation : Operation<ExecutionListExportModel, object>
    {
        public static void Operate(ExecutionListExportModel executionModel, IFileWriter fileWriter)
        {
            List<ExecutionDto> executionDTOs = new List<ExecutionDto>();
            foreach (Execution execution in executionModel.Executions)
            {
                executionDTOs.Add(new ExecutionDto
                {
                    K0 = execution.K0,
                    dt = execution.dt,
                    V = execution.V,
                    N = execution.N,
                    Percentage = execution.Percentage,
                    Error = execution.Error,
                    Method = nameof(execution.MethodType)
                });
            }

            string jsonContent = JsonConvert.SerializeObject(executionDTOs, Formatting.Indented);
            fileWriter.WriteToFile(executionModel.FilePath, jsonContent);
        }
    }




}
