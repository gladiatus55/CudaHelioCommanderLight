using CudaHelioCommanderLight.Dtos;
using CudaHelioCommanderLight.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace CudaHelioCommanderLight.Operations
{
    public static class ExportAsJsonOperation
    {
        public static void Operate(List<Execution> executions, string filePath)
        {
            List<ExecutionDto> executionDTOs = new List<ExecutionDto>();

            foreach (Execution execution in executions)
            {
                executionDTOs.Add(new ExecutionDto()
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

            using (StreamWriter file = File.CreateText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(file, executionDTOs);
            }
        }
    }
}
