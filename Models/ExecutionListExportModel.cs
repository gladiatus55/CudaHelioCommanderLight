using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CudaHelioCommanderLight.Models
{
    public class ExecutionListExportModel
    {
        public List<Execution> Executions { get; set; }
        public string FilePath { get; set; }
    }
}
