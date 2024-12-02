using System.Collections.Generic;

namespace CudaHelioCommanderLight.Models
{
    public class DisplayAmsHeatmapModel
    {
        public string? GraphName { get; set; }
        public List<ErrorStructure> Errors { get; set; }
        public string Tag { get; set; }
    }
}
