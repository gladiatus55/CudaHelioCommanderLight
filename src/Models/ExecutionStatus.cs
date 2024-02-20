using System.Collections.Generic;

namespace CudaHelioCommanderLight.Models
{
    public class ExecutionStatus
    {
        private List<ExecutionDetail> ActiveExecutions { get; }

        public List<ExecutionDetail> GetActiveExecutions()
        {
            return ActiveExecutions;
        }

        public ExecutionStatus()
        {
            ActiveExecutions = new List<ExecutionDetail>();
        }
    }
}
