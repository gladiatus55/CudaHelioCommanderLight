using System.Collections.Generic;

namespace CudaHelioCommanderLight.Models
{
    public class ExecutionStatus
    {
        private List<ExecutionDetail> activeExecutions { get; }

        public List<ExecutionDetail> GetActiveExecutions()
        {
            return activeExecutions;
        }

        public ExecutionStatus()
        {
            activeExecutions = new List<ExecutionDetail>();
        }
    }
}
