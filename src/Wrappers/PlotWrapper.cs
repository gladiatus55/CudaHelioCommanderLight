using CudaHelioCommanderLight.Interfaces;

public class PlotWrapper : IPlotWrapper
{
    private readonly ScottPlot.Plot _plot;

    public PlotWrapper(ScottPlot.Plot plot)
    {
        _plot = plot;
    }

    public void PlotScatter(double[] x, double[] y, int markerSize, System.Drawing.Color color, string label)
    {
        _plot.PlotScatter(x, y, markerSize: markerSize, color: color, label: label);
    }

    public void PlotHSpan(double x1, double x2, bool draggable, System.Drawing.Color color, float alpha)
    {
        _plot.PlotHSpan(x1: x1, x2: x2, draggable: draggable, color: color, alpha: alpha);
    }

    public void Title(string title) => _plot.Title(title);

    public void XLabel(string label) => _plot.XLabel(label);

    public void YLabel(string label) => _plot.YLabel(label);

    public void Legend() => _plot.Legend();
}
