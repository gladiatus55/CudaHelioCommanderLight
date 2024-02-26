using CudaHelioCommanderLight.Config;
using CudaHelioCommanderLight.Enums;
using CudaHelioCommanderLight.Exceptions;
using CudaHelioCommanderLight.Extensions;
using CudaHelioCommanderLight.Helpers;
using CudaHelioCommanderLight.Models;
using CudaHelioCommanderLight.Operations;
using CudaHelioCommanderLight.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CudaHelioCommanderLight
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<ExecutionDetail> ExecutionDetailList { get; set; }
        private string versionStr = "Version: 1.1.1l";
        private MetricsConfig metricsConfig;
        private PanelType currentlyDisplayedPanelType;
        private int executionDetailSelectedIdx = -1;
        private List<ErrorStructure> amsComputedErrors;
        private List<string> GeliosphereLibTypes;
        private List<string> GeliosphereLibBurgerRatios;
        private List<string> GeliosphereLibJGRRatios; 
        private MainWindowVm _mainWindowVm;
        public MainWindow()
        {
            InitializeComponent();

            metricsConfig = new MetricsConfig();
            MetricsUsedTB.Text = metricsConfig.ToString();
            _mainWindowVm = new MainWindowVm();
            metricsConfig.RegisterObserver(_mainWindowVm);

            DataContext = _mainWindowVm;

            SwitchPanels(PanelType.NONE);
            versionTb.Text = versionStr;

            InitializeAvailableGeliosphereLibs();
        }

        private void InitializeAvailableGeliosphereLibs()
        {
            var burgerTypeName = "Burger";
            var jgrTypeName = "JGR";
            GeliosphereLibTypes = new List<string>() { burgerTypeName, jgrTypeName };
            GeliosphereLibBurgerRatios = GetAvailableGeliosphereLibRatiosOperation.Operate(burgerTypeName);
            GeliosphereLibJGRRatios = GetAvailableGeliosphereLibRatiosOperation.Operate(jgrTypeName);

            geliosphereLibType.ItemsSource = GeliosphereLibTypes;
            geliosphereLibType.SelectedIndex = 0;
            geliosphereLibRatio.ItemsSource = GeliosphereLibBurgerRatios;
            geliosphereLibRatio.SelectedIndex = 0;

            geliosphereAllLibType.ItemsSource = GeliosphereLibTypes;
            geliosphereAllLibType.SelectedIndex = 0;
            geliosphereAllLibRatio.ItemsSource = GeliosphereLibBurgerRatios;
            geliosphereAllLibRatio.SelectedIndex = 0;
        }


        private void AboutUsButton_Click(object sender, RoutedEventArgs e)
        {
            string message = "Slovak Academy of Sciences\n\nDeveloped by: Martin Nguyen, Pavol Bobik\n\nCopyright 2023";
            MessageBox.Show(message, "About Us", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #region AMS
        private void RenderAmsGraph(AmsExecution amsExecution, ErrorStructure? errorStructure = null)
        {
            AmsGraphWpfPlot.Reset();
            var amsExecutionErrorModel = new AmsExecutionPltErrorModel()
            {
                AmsExecution = amsExecution,
                ErrorStructure = errorStructure,
                Plt = AmsGraphWpfPlot.plt,
                MetricsConfig = metricsConfig
            };

            RenderAmsErrorGraphOperation.Operate(amsExecutionErrorModel);
            AmsGraphWpfPlot.Render();
        }

        private void RenderAmsRatioGraph(AmsExecution amsExecution, ErrorStructure errorStructure = null)
        {
            AmsGraphRatioWpfPlot.Reset();
            var amsExecutionErrorModel = new AmsExecutionPltErrorModel()
            {
                AmsExecution = amsExecution,
                ErrorStructure = errorStructure,
                Plt = AmsGraphRatioWpfPlot.plt,
                MetricsConfig = metricsConfig
            };

            RenderAmsErrorRatioGraphOperation.Operate(amsExecutionErrorModel);
            AmsGraphRatioWpfPlot.Render();
        }

        private void DrawAmsHeatmapBtn_Click(object sender, RoutedEventArgs e)
        {
            if (amsComputedErrors.Count == 0)
            {
                MessageBox.Show("Empty errors!");
                return;
            }

            DisplayAmsHeatmapWindowOperation.Operate(new DisplayAmsHeatmapModel
            {
                GraphName = currentDisplayedAmsInvestigation?.FileName,
                Errors = amsComputedErrors,
                Tag = (string) ((Button)sender).Tag
            });
        }

        private void CompareWithLibrary(string libPath, LibStructureType libStructureType)
        {
            try
            {
                if (!Directory.Exists(libPath))
                {
                    MessageBox.Show("Library not found", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                AmsExecution exD = currentDisplayedAmsInvestigation;
                amsComputedErrors = new List<ErrorStructure>();


                var computedError = CompareLibraryOperation.Operate(new CompareLibraryModel()
                {
                    LibPath = libPath,
                    AmsExecution = exD,
                    MetricsConfig = metricsConfig
                }, libStructureType);
                if (libStructureType != LibStructureType.FILES_FORCEFIELD2023)
                {
                    amsComputedErrors.AddRange(computedError);
                }
                else
                {
                    GraphForceFieldWindow graphForceFieldWindow = new GraphForceFieldWindow(computedError);
                    graphForceFieldWindow.Show();
                    return;
                }

                if (sortByError)
                {
                    amsErrorsListBox.ItemsSource = amsComputedErrors.OrderBy(er => er.Error).ToList();
                }
                else
                {
                    amsErrorsListBox.ItemsSource = amsComputedErrors.OrderBy(er => er.MaxError).ToList();
                }

                currentDisplayedAmsInvestigation.AssignLowestValues(amsComputedErrors);
                dataGridAmsInner.Items.Refresh();
            }
            catch(WrongConfigurationException e)
            {
                MessageBox.Show(e.Message);
                OpenConfigurationWindow();
            }
        }

        private void CompareAllLoadedWithLib(string libPath, LibStructureType libStructureType)
        {
            try {
                if (!Directory.Exists(libPath))
                {
                    MessageBox.Show("Library not found", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                foreach (var exD in AmsExecutionList)
                {
                    amsComputedErrors = new List<ErrorStructure>();
                    var computedErrors = CompareLibraryOperation.Operate(new CompareLibraryModel()
                    {
                        LibPath = libPath,
                        AmsExecution = exD,
                        MetricsConfig = metricsConfig
                    }, libStructureType);

                    
                    amsComputedErrors.AddRange(computedErrors);

                    exD.AssignLowestValues(amsComputedErrors);
                    dataGridAmsInner.Items.Refresh();
                }

                ToggleExportAllAsCsvButton(true);
            }
            catch (WrongConfigurationException e)
            {
                MessageBox.Show(e.Message);
                ToggleExportAllAsCsvButton(false);
            }
        }

        private void ToggleExportAllAsCsvButton(bool value)
        {
            exportListAsCsvBtn.IsEnabled = value;
        }

        private void CompareWithHeliumLibBtn_Click(object sender, RoutedEventArgs e)
        {
            var libPath = @"libFiles\lib-helium";
            CompareWithLibrary(libPath, LibStructureType.DIRECTORY_SEPARATED);
        }

        private void CompareWithLibBtn_Click(object sender, RoutedEventArgs e)
        {
            var libPath = @"libFiles\lib-proton";
            CompareWithLibrary(libPath, LibStructureType.DIRECTORY_SEPARATED);
        }

        private void CompareWithGeliosphereLibBtn_Click(object sender, RoutedEventArgs e)
        {
            if (geliosphereLibRatio.SelectedItem == null)
            {
                MessageBox.Show("Library not found", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var libtype = geliosphereLibType.SelectedItem.ToString();
            var libRatio = geliosphereLibRatio.SelectedItem.ToString().Replace(',', '.');
            var libPath = $"libFiles\\lib-geliosphere-{libtype}-{libRatio}";
            CompareWithLibrary(libPath, LibStructureType.FILES_SOLARPROP_LIB);
        }

        private void CompareWithForceFieldLibBtn_Copy_Click(object sender, RoutedEventArgs e)
        {
            var libPath = @"libFiles\lib-forcefield2023";
            CompareWithLibrary(libPath, LibStructureType.FILES_FORCEFIELD2023_COMPARISION);
        }

        private void CompareAllLoadedWithHeLibBtn_Click(object sender, RoutedEventArgs e)
        {
            var libPath = @"libFiles\lib-helium";
            CompareAllLoadedWithLib(libPath, LibStructureType.DIRECTORY_SEPARATED);
        }

        private async void CompareAllLoadedWithLibBtn_Click(object sender, RoutedEventArgs e)
        {
            var libPath = @"libFiles\lib-proton";
            CompareAllLoadedWithLib(libPath, LibStructureType.DIRECTORY_SEPARATED);
        }

        private async void CompareAllLoadedWitGeliosphereLibBtn_Click(object sender, RoutedEventArgs e)
        {
            if (geliosphereAllLibRatio.SelectedItem == null)
            {
                MessageBox.Show("Library not found", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var libtype = geliosphereAllLibType.SelectedItem.ToString();
            var libRatio = geliosphereAllLibRatio.SelectedItem.ToString().Replace(',', '.');
            var libPath = $"libFiles\\lib-geliosphere-{libtype}-{libRatio}";
            CompareAllLoadedWithLib(libPath, LibStructureType.FILES_SOLARPROP_LIB);
        }

        private void CompareAllLoadedWithForceFieldLibBtn_Click(object sender, RoutedEventArgs e)
        {
            var libPath = @"libFiles\lib-forcefield2023";
            CompareAllLoadedWithLib(libPath, LibStructureType.FILES_FORCEFIELD2023_COMPARISION);
        }

        private void AmsErrorsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var error = (ErrorStructure)amsErrorsListBox.SelectedItem;
            if (error == null)
            {
                return;
            }
            AmsExecution exD = (AmsExecution)dataGridAmsInner.SelectedItem;

            RenderAmsGraph(exD, error);
            RenderAmsRatioGraph(exD, error);
            openedLibraryNameTb.Text = error.DisplayName;
        }

        private bool sortByError = true;

        private void AmsApplyFilterBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(((Button)sender).Tag?.ToString()))
            {
                var str = ((Button)sender).Tag.ToString();

                if (str == "error")
                {
                    sortByError = true;
                }
                else
                {
                    sortByError = false;
                }
            }

            var filterV = amsFilterVTb.Text;
            var filterK0 = amsFilterK0Tb.Text;

            var filteredList = amsComputedErrors.ToList();

            if (!string.IsNullOrEmpty(filterV))
            {
                bool success = MainHelper.TryConvertToDouble(filterV, out double V);
                filteredList = filteredList.Where(er => er.V == (int)V).ToList();
            }

            if (!string.IsNullOrEmpty(filterK0))
            {
                bool success = MainHelper.TryConvertToDouble(filterK0, out double K0);
                filteredList = filteredList.Where(er => AreDoubleValuesEqual(er.K0, K0)).ToList();
            }

            //amsErrorsListBox.Items.Clear();
            if (sortByError)
            {
                amsErrorsListBox.ItemsSource = filteredList.OrderBy(er => er.Error).ToList();
            }
            else
            {
                amsErrorsListBox.ItemsSource = filteredList.OrderBy(er => er.MaxError).ToList();
            }
        }

        private bool AreDoubleValuesEqual(double d1, double d2)
        {
            return Math.Abs(d1 - d2) < 0.0000001;
        }

        private void GeliosphereLibType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (geliosphereLibType.SelectedIndex)
            {
                case 0: // burger
                    geliosphereLibRatio.ItemsSource = GeliosphereLibBurgerRatios;
                    geliosphereLibRatio.SelectedIndex = 0;
                    break;
                case 1: // JGR
                    geliosphereLibRatio.ItemsSource = GeliosphereLibJGRRatios;
                    geliosphereLibRatio.SelectedIndex = 0;
                    break;
            }
        }

        private void GeliosphereAllLibType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (geliosphereAllLibType.SelectedIndex)
            {
                case 0: // burger
                    geliosphereAllLibRatio.ItemsSource = GeliosphereLibBurgerRatios;
                    geliosphereAllLibRatio.SelectedIndex = 0;
                    break;
                case 1: // JGR
                    geliosphereAllLibRatio.ItemsSource = GeliosphereLibJGRRatios;
                    geliosphereAllLibRatio.SelectedIndex = 0;
                    break;
            }
        }

        private void ExportAsCsvBtn_Click(object sender, RoutedEventArgs e)
        {
            ExportAsCsvOperation.Operate((IEnumerable<ErrorStructure>)amsErrorsListBox.ItemsSource);
        }
        #endregion AMS

        private void OpenExplorerButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();

            System.Windows.Forms.DialogResult dialogResult = folderDialog.ShowDialog();

            if (dialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            string selectedFolderPath = folderDialog.SelectedPath;

            if (string.IsNullOrEmpty(selectedFolderPath))
            {
                return;
            }

            SwitchPanels(PanelType.STATUS_CHECKER);

            ExecutionStatus executionStatus = MainHelper.ExtractOfflineExecStatus(selectedFolderPath);

            ExecutionDetailList = new ObservableCollection<ExecutionDetail>(executionStatus.activeExecutions);
            ActiveCalculationsDataGrid.ItemsSource = ExecutionDetailList;
        }

        private void SwitchPanels(PanelType panelType)
        {
            this.ExplorerMainpanel.Visibility = Visibility.Hidden;
            this.ExplorerLeftPanel.Visibility = Visibility.Hidden;
            this.ExplorerRightPanel.Visibility = Visibility.Hidden;
            this.StatusCheckerMainPanel.Visibility = Visibility.Hidden;
            this.StatusCheckerGridPanelDetail.Visibility = Visibility.Hidden;
            this.AmsInvestigationPanel.Visibility = Visibility.Hidden;
            this.AmsInvestigationDetailPanel.Visibility = Visibility.Hidden;

            switch (panelType)
            {
                case PanelType.STATUS_CHECKER_DETAIL:
                    this.StatusCheckerGridPanelDetail.Visibility = Visibility.Visible;
                    break;
                case PanelType.STATUS_CHECKER:
                    this.StatusCheckerMainPanel.Visibility = Visibility.Visible;
                    break;
                case PanelType.EXPLORER:
                    this.ExplorerMainpanel.Visibility = Visibility.Visible;
                    this.ExplorerRightPanel.Visibility = Visibility.Visible;
                    this.ExplorerLeftPanel.Visibility = Visibility.Visible;
                    break;
                case PanelType.AMS_INVESTIGATION:
                    this.AmsInvestigationPanel.Visibility = Visibility.Visible;
                    break;
                case PanelType.AMS_INVESTIGATION_DETAIL:
                    this.AmsInvestigationDetailPanel.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }

            currentlyDisplayedPanelType = panelType;

        }

        private ObservableCollection<AmsExecution> AmsExecutionList { get; set; }
        private AmsExecution currentDisplayedAmsInvestigation;

        private void RmsComputeModeBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Multiselect = true;
            open.Title = "Select files";

            if (open.ShowDialog() == false)
            {
                return;
            }

            SwitchPanels(PanelType.AMS_INVESTIGATION);
            AmsExecutionDetail executionStatus = MainHelper.ExtractMultipleOfflineStatus(open.FileNames.ToList());

            AmsExecutionList = new ObservableCollection<AmsExecution>(executionStatus.AmsExecutions);
            dataGridAmsInner.ItemsSource = AmsExecutionList;
        }

        private void CloseMainPanelButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentlyDisplayedPanelType == PanelType.EXPLORER)
            {
                SwitchPanels(PanelType.STATUS_CHECKER_DETAIL);
            }
            else if (currentlyDisplayedPanelType == PanelType.STATUS_CHECKER_DETAIL)
            {
                SwitchPanels(PanelType.STATUS_CHECKER);
            }
            else if (currentlyDisplayedPanelType == PanelType.STATUS_CHECKER || currentlyDisplayedPanelType == PanelType.AMS_INVESTIGATION)
            {
                SwitchPanels(PanelType.NONE);
            }
            else if (currentlyDisplayedPanelType == PanelType.AMS_INVESTIGATION_DETAIL)
            {
                SwitchPanels(PanelType.AMS_INVESTIGATION);
            }
        }

        private void CreateErrorGraphBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                List<ExecutionDetail> selectedExecutionDetails = new List<ExecutionDetail>();


                if (fileDialog.ShowDialog() == false)
                {
                    return;
                }
                string filePath = fileDialog.FileName;
                bool dataExtractSuccess = MainHelper.ExtractOutputDataFile(filePath, out OutputFileContent outputFileContent);

                if (!dataExtractSuccess)
                {
                    MessageBox.Show("Cannot read data values from the input file.");
                    return;
                }

                foreach (ExecutionDetail executionDetail in ActiveCalculationsDataGrid.Items)
                {
                    if (executionDetail.IsSelected)
                    {
                        selectedExecutionDetails.Add(executionDetail);

                        foreach (Execution execution in executionDetail.Executions)
                        {
                            ExecutionHelper.InitializeOutput1e3BinDataFromOnlineDir(execution);

                            if (execution.StandardDeviatons != null)
                            {
                                try
                                {
                                    execution.ComputeError(outputFileContent, metricsConfig); // If error in computeError, this was changed recently
                                }
                                catch (ArgumentOutOfRangeException ex)
                                {
                                    MessageBox.Show(ex.ToString());
                                    return;
                                }
                            }
                        }
                    }
                }

                RenderGraphOfErrors(selectedExecutionDetails);
            }
            catch(WrongConfigurationException ex)
            {
                MessageBox.Show(ex.Message);
                OpenConfigurationWindow();
            }
        }

        private void RenderGraphOfErrors(List<ExecutionDetail> selectedExecutionDetails)
        {
            List<System.Drawing.Color> colorList = new List<System.Drawing.Color>()
            {
                System.Drawing.Color.Red,
                System.Drawing.Color.Green,
                System.Drawing.Color.Blue,
                System.Drawing.Color.Aqua,
                System.Drawing.Color.Orange
            };
            var plt = new ScottPlot.Plot(600, 400);

            bool firstRun = true;
            double minY = 0.0;
            double maxY = 0.0;

            List<string> vAndKToIdx = new List<string>();

            //foreach (GraphInfo graphInfo in loadedGraphs)
            for (int idx = 0; idx < selectedExecutionDetails.Count; idx++)
            {
                List<Execution> nLowestExecutions = selectedExecutionDetails[idx].GetLowestExecutions(10);

                foreach (Execution e in nLowestExecutions)
                {
                    string stringify = "V=" + e.V + "K0=" + e.K0;

                    if (!vAndKToIdx.Contains(stringify))
                    {
                        vAndKToIdx.Add(stringify);
                    }
                }

                double[] x = nLowestExecutions.Select(e => (double)vAndKToIdx.IndexOf("V=" + e.V + "K0=" + e.K0)).ToArray();
                double[] y = nLowestExecutions.Select(e => e.ErrorValue).ToArray();

                plt.PlotScatter(x, y, markerSize: 5, lineWidth: 0, color: colorList[idx], label: selectedExecutionDetails[idx].FolderName);
                firstRun = false;
            }

            plt.XTicks(Enumerable.Range(0, vAndKToIdx.Count).Select(a => (double)a).ToArray(), vAndKToIdx.ToArray());

            plt.Title("Plotted errors");
            plt.YLabel("Error");
            plt.XLabel("Index");
            plt.Legend();
            var plotViewer = new ScottPlot.WpfPlotViewer(plt);

            plotViewer.Show();
        }

        private void ActiveCalculationsDataGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Button b = sender as Button;

            DataGrid dataGrid = sender as DataGrid;
            DataGridRow row = findParentOfType<DataGridRow>(e.OriginalSource as DependencyObject);

            if (dataGrid != null && row != null)
            {
                //the row DataContext is the selected item
                if (dataGrid.SelectedItems.Contains(row.DataContext))
                {
                    dataGrid.SelectedItems.Remove(row.DataContext);
                    //mark event as handled so that datagrid does not
                    //just select it again on the current click.
                    e.Handled = true;
                }
            }
        }

        //This helper is used to find your target, which in your case is a DataGridRow.
        //In general, if you continue with WPF, I would suggest adding
        //this very handy method to your extensions library.
        private T findParentOfType<T>(DependencyObject source) where T : DependencyObject
        {
            T ret = default(T);
            UIElement parent = VisualTreeHelper.GetParent(source) as UIElement;

            if (parent != null)
            {
                ret = parent as T ?? findParentOfType<T>(parent) as T;
            }
            return ret;
        }

        private void ImageTeplateRightMouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            ImageViewer imageViewer = new ImageViewer((string)((Image)sender).Tag);
            imageViewer.ShowDialog();
        }

        private void AmsSelectDetailButton_Click(object sender, RoutedEventArgs e)
        {
            AmsExecution exD = (AmsExecution)dataGridAmsInner.SelectedItem;
            var executionDetailSelectedIdx = AmsExecutionList.IndexOf(exD);
            currentDisplayedAmsInvestigation = exD;
            SwitchPanels(PanelType.AMS_INVESTIGATION_DETAIL);

            RenderAmsGraph(exD);
            amsComputedErrors = new List<ErrorStructure>();
            amsErrorsListBox.ItemsSource = new List<ErrorStructure>();
        }

        private void ConfigureMetricsBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenConfigurationWindow();
        }

        private void OpenConfigurationWindow()
        {
            var configWindowResult = OpenConfigurationWindowOperation.Operate(metricsConfig);

            this.metricsConfig = configWindowResult.MetricsConfig;
            this.MetricsUsedTB.Text = metricsConfig.ToString();

            if (currentlyDisplayedPanelType == PanelType.AMS_INVESTIGATION_DETAIL && configWindowResult.HasChanged)
            {
                CompareWithLibBtn_Click(null, null);
            }
        }

        private void DrawForceField2023RmsBtn_Click(object sender, RoutedEventArgs e)
        {
            var libPath = @"libFiles\lib-forcefield2023";
            CompareWithLibrary(libPath, LibStructureType.FILES_FORCEFIELD2023);
        }

        private void DgItemCheckboxClicked(object sender, RoutedEventArgs e)
        {
            if (ActiveCalculationsDataGrid.SelectedIndex == -1)
            {
                return;
            }

            CheckBox clickedCb = (CheckBox)sender;
            ExecutionDetail selectedExecutionDetail = (ExecutionDetail)ActiveCalculationsDataGrid.SelectedItem;

            selectedExecutionDetail.IsSelected = clickedCb.IsChecked == true;
        }

        private void CalculationDetailButton_Click(object sender, RoutedEventArgs e)
        {
            ExecutionDetail exD = (ExecutionDetail)ActiveCalculationsDataGrid.SelectedItem;
            executionDetailSelectedIdx = ExecutionDetailList.IndexOf(exD);
            SwitchPanels(PanelType.STATUS_CHECKER_DETAIL);

            dgInner.ItemsSource = exD.Executions;
        }

        private void DetailBtn_Click(object sender, RoutedEventArgs e)
        {
            Execution openedExecution = GetCurrentlyOpenedExecution();

            if (string.IsNullOrEmpty(openedExecution.LocalDirPath))
            {
                MessageBox.Show("Sorry, detail view for this execution is not available");
                return;
            }

            var windowExecutionModel = new WindowExecutionModel
            {
                MainWindow = this,
                Execution = openedExecution
            };

            LoadExplorerDataOperation.Operate(windowExecutionModel);
            LoadSpectraImagesToCanvas.Operate(windowExecutionModel);

            SwitchPanels(PanelType.EXPLORER);
        }

        private Execution GetCurrentlyOpenedExecution()
        {
            if (dgInner.SelectedIndex != -1)
            {
                return (Execution)dgInner.SelectedItem;
            }

            return null;
        }

        private void ExportJsonBtn_Click(object sender, RoutedEventArgs e)
        {
            ExecutionDetail executionDetail = ExecutionDetailList[executionDetailSelectedIdx];

            if (executionDetail == null)
            {
                return;
            }

            SaveFileDialog fileDialog = new SaveFileDialog();

            fileDialog.Filter = "JSON File|*.json";
            fileDialog.Title = "Save JSON File";
            fileDialog.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (fileDialog.FileName != "")
            {
                var exportModel = new ExecutionListExportModel
                {
                    Executions = executionDetail.Executions,
                    FilePath = fileDialog.FileName
                };
                ExportAsJsonOperation.Operate(exportModel);
            }
        }

        private void ComputeErrorBtn_Click(object sender, RoutedEventArgs e)
        {
            ExecutionDetail executionDetail = ExecutionDetailList[executionDetailSelectedIdx];

            OpenFileDialog fileDialog = new OpenFileDialog();

            if (fileDialog.ShowDialog() == true)
            {
                string filePath = fileDialog.FileName;
                
                bool dataExtractSuccess = MainHelper.ExtractOutputDataFile(filePath, out OutputFileContent outputFileContent);

                if (!dataExtractSuccess)
                {
                    MessageBox.Show("Cannot read data values from the input file.");
                    return;
                }

                LoadedOutputFileChecker dataChecker = new LoadedOutputFileChecker(outputFileContent);

                dataChecker.ShowDialog();

                foreach (Execution execution in executionDetail.Executions)
                {
                    ExecutionHelper.InitializeOutput1e3BinDataFromOnlineDir(execution);

                    if (execution.StandardDeviatons != null)
                    {
                        try
                        {
                            execution.ComputeError(outputFileContent, metricsConfig); // If error in computeError, this was changed recently
                        }
                        catch (ArgumentOutOfRangeException ex)
                        {
                            MessageBox.Show(ex.ToString());
                            return;
                        }
                    }
                }
                dgInner.Items.Refresh();
            }
        }

        private void Spe1e3CanvasFit30gevCanvas_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            DisplayImageOperation.Operate(spe1e3Fit30gevCanvas.Background);
        }

        private void Spe1e3CanvasFitCanvas_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            DisplayImageOperation.Operate(spe1e3FitCanvas.Background);
        }

        private void Spe1e3Canvas_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            DisplayImageOperation.Operate(spe1e3Canvas.Background);
        }

        private void Spe1e3nCanvas_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            DisplayImageOperation.Operate(spe1e3nCanvas.Background);
        }

        private void Spe1e3Canvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mainCanvas.Background = spe1e3Canvas.Background;
        }

        private void Spe1e3nCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mainCanvas.Background = spe1e3nCanvas.Background;
        }

        private void Spe1e3Fit30gevCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mainCanvas.Background = spe1e3Fit30gevCanvas.Background;
        }

        private void Spe1e3FitCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mainCanvas.Background = spe1e3FitCanvas.Background;
        }

        private void DrawHeatmapBtn_Click(object sender, RoutedEventArgs e)
        {
            HeatMapGraph heatMap = new HeatMapGraph();
            heatMap.Show();

            ExecutionDetail executionDetail = ExecutionDetailList[executionDetailSelectedIdx];

            int xSize = executionDetail.paramK0.Count;
            int ySize = executionDetail.paramV.Count;

            if (xSize < 2 || ySize < 2)
            {
                MessageBox.Show("Cannot make map");
                return;
            }

            HeatMapGraph.HeatPoint[,] heatPoints = new HeatMapGraph.HeatPoint[xSize, ySize];

            for (int i = 0; i < xSize; i++)
            {
                for (int j = 0; j < ySize; j++)
                {
                    double k0 = executionDetail.paramK0[i];
                    double V = executionDetail.paramV[j];
                    Execution ex = executionDetail.GetExecutionByParam(V, k0);

                    if (ex == null)
                    {
                        heatPoints[i, j] = new HeatMapGraph.HeatPoint(k0, V, double.NaN);
                        continue;
                    }

                    double error = ex.ErrorValue;

                    heatPoints[i, j] = new HeatMapGraph.HeatPoint(k0, V, error);
                }
            }

            heatMap.SetPoints(heatPoints, xSize, ySize);
            heatMap.Render();

            return;
        }

        private void ExportListAsCsvBtn_Click(object sender, RoutedEventArgs e)
        {
            ExportListAsCsvOperation.Operate(AmsExecutionList);
        }
    }
}
