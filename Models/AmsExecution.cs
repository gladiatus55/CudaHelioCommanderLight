using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CudaHelioCommanderLight.Models
{
    public class AmsExecution
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public List<double> TKin { get; set; }
        public List<double> Spe1e3 { get; set; }
        public string LowestRMSErrorStr { 
            get
            {
                if (LowestRMSError == default)
                {
                    return string.Empty;
                }
                return $"{LowestRMSError}% ({LowestRMSErrorFile})";
            } 
        }
        public string MinimalMaxErrorStr
        {
            get
            {
                if (MinimalMaxError == default)
                {
                    return string.Empty;
                }
                return $"{MinimalMaxError}% ({MinimalMaxErrorFile})";
            }
        }
        public string LowestRMSError { get; set; }
        public string MinimalMaxError { get; set; }
        public string LowestRMSErrorFile { get; set; }
        public string MinimalMaxErrorFile { get; set; }
    }
}
