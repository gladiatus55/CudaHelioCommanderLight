using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CudaHelioCommanderLight.Operations
{
    public static class DisplayImageOperation
    {
        public static void Operate(Brush imgBrush)
        {
            ImageViewer imageViewer = new ImageViewer(imgBrush);
            imageViewer.ShowDialog();
        }
    }
}
