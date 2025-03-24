namespace CudaHelioCommanderLight.Interfaces
{
    public interface IPlotWrapper
    {
        void PlotScatter(double[] x, double[] y, int markerSize, System.Drawing.Color color, string label);
        void PlotHSpan(double x1, double x2, bool draggable, System.Drawing.Color color, float alpha);
        void Title(string title);
        void XLabel(string label);
        void YLabel(string label);
        void Legend();
    }
}
