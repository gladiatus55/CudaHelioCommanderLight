using CudaHelioCommanderLight.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CudaHelioCommanderLight.Models
{
    public class CompareLibraryModel
    {
        public string LibPath { get; set; }
        public AmsExecution AmsExecution { get; set; }
        public MetricsConfig MetricsConfig { get; set; } // TODO: Remove when singleton
    }
}
