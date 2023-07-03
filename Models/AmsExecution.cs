using System.Collections.Generic;

namespace CudaHelioCommanderLight.Models
{
    public class AmsExecution
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public List<double> TKin { get; set; }
        public List<double> Spe1e3 { get; set; }
        public string LowRMSError { get; set; }
        public string MinError { get; set; }
    }
}
