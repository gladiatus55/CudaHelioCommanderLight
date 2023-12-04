using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CudaHelioCommanderLight
{
    /// <summary>
    /// Interaction logic for ImageViewer.xaml
    /// </summary>
    public partial class ImageViewer : Window
    {
        public ImageViewer(string imagePath)
        {
            InitializeComponent();

            if (!File.Exists(imagePath))
            {
                MessageBox.Show(string.Format("Image path {0} not found!", imagePath));
                this.Close();
            }

            this.mainCanvas.Background = new ImageBrush(new BitmapImage(new Uri(imagePath)));
        }

        public ImageViewer(Brush image)
        {
            InitializeComponent();

            this.mainCanvas.Background = image;
        }
    }
}
