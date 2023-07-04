using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CudaHelioCommanderLight.Operations
{
    public abstract class Operation<TRequest, TResponse>
    {
        public virtual TResponse Operate(TRequest request = default)
        {
            var response = default(TResponse);
            return response;
        }
    }
}
