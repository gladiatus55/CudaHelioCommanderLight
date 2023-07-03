using System.Collections.Generic;

namespace CudaHelioCommanderLight.Models
{
    public class AmsExecutionDetail
    {
        public List<AmsExecution> AmsExecutions { get; set; }

        public AmsExecutionDetail()
        {
            AmsExecutions = new List<AmsExecution>();
        }
    }
}
