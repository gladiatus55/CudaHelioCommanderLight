using System.Windows.Media;

namespace CudaHelioCommanderLight.Operations
{
    public class DisplayImageOperation : Operation<Brush, object>
    {
        public static new void Operate(Brush imgBrush)
        {
            ImageViewer imageViewer = new ImageViewer(imgBrush);
            imageViewer.ShowDialog();
        }
    }
}
