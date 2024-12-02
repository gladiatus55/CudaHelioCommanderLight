using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CudaHelioCommanderLight.Helpers
{
    public class ShapeDrawing
    {
        public static void DrawCircle(Canvas canvas, double centerX, double centerY, double radius, SolidColorBrush brush, ArrayList drawnObjects)
        {
            Ellipse circle = new Ellipse();
            circle.Stroke = Brushes.Cyan;
            circle.SetValue(Canvas.LeftProperty, centerX - radius);
            circle.SetValue(Canvas.TopProperty, centerY - radius);
            circle.Width = radius + radius;
            circle.Height = radius + radius;

            canvas.Children.Add(circle);
            int zindex = canvas.Children.Count;
            Canvas.SetZIndex(circle, zindex);

            drawnObjects.Add(circle);
        }

        public static void DrawSolidRectangle(Canvas canvas, double top, double left, double width, double height, Brush brush, ArrayList drawnObjects)
        {
            Rectangle rectangle = new Rectangle()
            {
                Width = width,
                Height = height,
                Fill = brush,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
            };

            Canvas.SetTop(rectangle, top);
            Canvas.SetLeft(rectangle, left);

            canvas.Children.Add(rectangle);
            drawnObjects.Add(rectangle);
        }

        public static void DrawText(Canvas canvas, string text, double top, double left, Brush brush, ArrayList drawnObjects, bool bold = false, int rotate = 0)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Foreground = brush;
            RotateTransform rotateTransform = new RotateTransform(rotate);
            textBlock.RenderTransform = rotateTransform;
            textBlock.FontWeight = bold ? FontWeights.Bold : FontWeights.Regular;
            Canvas.SetLeft(textBlock, left);
            Canvas.SetTop(textBlock, top);
            canvas.Children.Add(textBlock);
            drawnObjects.Add(textBlock);
        }
    }
}
