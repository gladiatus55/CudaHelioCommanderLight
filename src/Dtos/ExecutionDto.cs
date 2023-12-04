
namespace CudaHelioCommanderLight.Dtos
{
    public class ExecutionDto
    {
        public double K0 { get; set; }
        public double dt { get; set; }
        public double V { get; set; }
        public double N { get; set; }
        public int Percentage { get; set; }
        public double Error { get; set; }
        public string Method { get; set; }
    }
}
