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

    public abstract class Operation<TRequest>
    {
        public virtual void Operate(TRequest request = default)
        {
        }
    }
}
