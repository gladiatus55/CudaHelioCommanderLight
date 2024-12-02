using System.Collections.Generic;

namespace CudaHelioCommanderLight.Models
{
    public class ExecutionListExportModel
    {
        public List<Execution> Executions { get; set; }
        public string FilePath { get; set; }
    }
}
