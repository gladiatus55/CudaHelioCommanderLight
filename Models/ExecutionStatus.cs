using System.Collections.Generic;

namespace CudaHelioCommanderLight.Models
{
    public class ExecutionStatus
    {
        public List<ExecutionDetail> activeExecutions;

        public ExecutionStatus()
        {
            activeExecutions = new List<ExecutionDetail>();
        }
    }
}
