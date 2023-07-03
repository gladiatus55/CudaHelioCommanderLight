
using CudaHelioCommanderLight.Constants;
using CudaHelioCommanderLight.Helpers;
using CudaHelioCommanderLight.Models;
using global::CudaHelioCommanderLight.Constants;
using global::CudaHelioCommanderLight.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CudaHelioCommanderLight.Operations
{
        public class WindowExplorer
        {
            private MainWindow mainWindow;
            private Execution execution;
            public WindowExplorer(MainWindow mainWindow, Execution execution)
            {
                this.mainWindow = mainWindow;
                this.execution = execution;
            }

            public void LoadDataToUI()
            {
                mainWindow.explorerDetailMethodTB.Text = execution.MethodType.ToString();
                mainWindow.explorerDetailK0TB.Text = execution.K0.ToString();
                mainWindow.explorerDetailDtTB.Text = execution.dt.ToString();
                mainWindow.explorerDetailVTB.Text = execution.V.ToString();
                mainWindow.explorerDetailNTB.Text = execution.N.ToString();

                DisplayGraphs();
            }

            private void DisplayGraphs()
            {
                string localDirPath = execution.LocalDirPath;

                string localGraphSpe1e3FilePath = Path.Combine(localDirPath, GlobalFilesToDownload.DetailSpe1e3GraphFile);
                string localGraphSpe1e3NFilePath = Path.Combine(localDirPath, GlobalFilesToDownload.DetailSpe1e3NGraphFile);
                string localGraphSpe1e3FitFilePath = Path.Combine(localDirPath, GlobalFilesToDownload.DetailSpe1e3FitGraphFile);
                string localGraphSpe1e3Fit30FilePath = Path.Combine(localDirPath, GlobalFilesToDownload.DetailSpe1e3Fit30GraphFile);


                if (File.Exists(localGraphSpe1e3NFilePath))
                {
                    mainWindow.spe1e3nCanvas.Background = new ImageBrush(new BitmapImage(new Uri(localGraphSpe1e3NFilePath)));
                }
                else
                {
                    mainWindow.spe1e3nCanvas.Background = new SolidColorBrush(Colors.Transparent);
                }

                if (File.Exists(localGraphSpe1e3FilePath))
                {
                    mainWindow.spe1e3Canvas.Background = new ImageBrush(new BitmapImage(new Uri(localGraphSpe1e3FilePath)));
                    mainWindow.mainCanvas.Background = new ImageBrush(new BitmapImage(new Uri(localGraphSpe1e3FilePath)));
                }
                else
                {
                    mainWindow.spe1e3Canvas.Background = new SolidColorBrush(Colors.Transparent);
                    mainWindow.mainCanvas.Background = new SolidColorBrush(Colors.Transparent);
                }

                if (File.Exists(localGraphSpe1e3Fit30FilePath))
                {
                    mainWindow.spe1e3Fit30gevCanvas.Background = new ImageBrush(new BitmapImage(new Uri(localGraphSpe1e3Fit30FilePath)));
                }
                else
                {
                    mainWindow.spe1e3Fit30gevCanvas.Background = new SolidColorBrush(Colors.Transparent);
                }

                if (File.Exists(localGraphSpe1e3FitFilePath))
                {
                    mainWindow.spe1e3FitCanvas.Background = new ImageBrush(new BitmapImage(new Uri(localGraphSpe1e3FitFilePath)));
                }
                else
                {
                    mainWindow.spe1e3FitCanvas.Background = new SolidColorBrush(Colors.Transparent);
                }
            }

            public void PrepareAndDisplayImageViewer(Brush imgBrush)
            {
                ImageViewer imageViewer = new ImageViewer(imgBrush);
                imageViewer.ShowDialog();
            }
        }
    }
