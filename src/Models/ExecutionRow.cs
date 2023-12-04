
namespace CudaHelioCommanderLight.Models
{
    public class ExecutionRow
    {
        public double TKin { get; set; }
        public double Count { get; set; }
        public double Spectra { get; set; }
        public double StandardDeviation { get; set; }
        public double WHLIS { get; set; }
        public double Other { get; set; }

        public ExecutionRow()
        {

        }
    }
}
