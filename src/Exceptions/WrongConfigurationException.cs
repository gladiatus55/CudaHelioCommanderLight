using System;

namespace CudaHelioCommanderLight.Exceptions
{
    public class WrongConfigurationException : Exception
    {
        public WrongConfigurationException(string message)
        : base(message)
        {
        }
    }
}
