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
using CudaHelioCommanderLight.MainWindowServices;

namespace CudaHelioCommanderLight
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<ExecutionDetail> ExecutionDetailList { get; set; }
        private string versionStr = "Version: 1.1.1l";
        private PanelType currentlyDisplayedPanelType;
        private int executionDetailSelectedIdx = -1;
        private List<ErrorStructure> amsComputedErrors;
        private List<string> GeliosphereLibTypes;
        private List<string> GeliosphereLibBurgerRatios;
        private List<string> GeliosphereLibJGRRatios; 
        private MainWindowVm _mainWindowVm;

        private ButtonService _buttonService;
        private RenderingService _renderingService;
        private HeatMapService _heatMapService;
        private CompareService _compareService;
        public MainWindow()
        {
            InitializeComponent();

            MetricsUsedTB.Text = MetricsConfig.GetInstance().ToString();
            _mainWindowVm = new MainWindowVm();
            MetricsConfig.GetInstance().RegisterObserver(_mainWindowVm);

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
            
            _buttonService = new ButtonService();
            _renderingService = new RenderingService();
            _heatMapService = new HeatMapService();
            _compareService = new CompareService();
        }


        private void AboutUsButton_Click(object sender, RoutedEventArgs e)
        {
            _buttonService.AboutUsButton_Click(sender, e);
        }

        #region AMS
        private void RenderAmsGraph(AmsExecution amsExecution, ErrorStructure? errorStructure = null)
        {
            _renderingService.RenderAmsGraph(amsExecution,AmsGraphWpfPlot, errorStructure);
        }

        private void DrawAmsHeatmapBtn_Click(object sender, RoutedEventArgs e)
        {
            _heatMapService.DrawAmsHeatmapBtn_Click(currentDisplayedAmsInvestigation?.FileName, amsComputedErrors, (string) ((Button)sender).Tag);
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
        
        private void CompareWithLib_Click(object sender, RoutedEventArgs e)
        {
            var (libPath, libStructureType) = _compareService.CompareWithLib((string)((Button)sender).Tag,
                geliosphereLibRatio, geliosphereLibType);
            
            if(((Button)sender).Tag != null && libPath != null)
            {
                CompareWithLibrary(libPath, libStructureType);
            }
        }

        private void CompareAllLoadedWithLib_Click(object sender, RoutedEventArgs e)
        {
            var (libPath, libStructureType) = _compareService.CompareWithLib((string)((Button)sender).Tag,
                geliosphereAllLibRatio, geliosphereAllLibType);

            if (((Button)sender).Tag != null && libPath != null)
            {
                CompareAllLoadedWithLib(libPath, libStructureType);
            }
        }

        private void AmsErrorsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var error = _renderingService.AmsErrorsListBox_SelectionChanged((ErrorStructure)amsErrorsListBox.SelectedItem, AmsGraphWpfPlot, AmsGraphRatioWpfPlot, (AmsExecution)dataGridAmsInner.SelectedItem );
            if (error == null)
            {
                return;
            }
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
                MainHelper.TryConvertToDouble(filterV, out double V);
                filteredList = filteredList.Where(er => er.V == (int)V).ToList();
            }

            if (!string.IsNullOrEmpty(filterK0))
            {
                MainHelper.TryConvertToDouble(filterK0, out double K0);
                filteredList = filteredList.Where(er => AreDoubleValuesEqual(er.K0, K0)).ToList();
            }
            
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

            ExecutionDetailList = new ObservableCollection<ExecutionDetail>(executionStatus.GetActiveExecutions());
            ActiveCalculationsDataGrid.ItemsSource = ExecutionDetailList;
        }

        private void SwitchPanels(PanelType panelType)
        {
            ExplorerMainpanel.Visibility = Visibility.Hidden;
            ExplorerLeftPanel.Visibility = Visibility.Hidden;
            ExplorerRightPanel.Visibility = Visibility.Hidden;
            StatusCheckerMainPanel.Visibility = Visibility.Hidden;
            StatusCheckerGridPanelDetail.Visibility = Visibility.Hidden;
            AmsInvestigationPanel.Visibility = Visibility.Hidden;
            AmsInvestigationDetailPanel.Visibility = Visibility.Hidden;

            switch (panelType)
            {
                case PanelType.STATUS_CHECKER_DETAIL:
                    StatusCheckerGridPanelDetail.Visibility = Visibility.Visible;
                    break;
                case PanelType.STATUS_CHECKER:
                    StatusCheckerMainPanel.Visibility = Visibility.Visible;
                    break;
                case PanelType.EXPLORER:
                    ExplorerMainpanel.Visibility = Visibility.Visible;
                    ExplorerRightPanel.Visibility = Visibility.Visible;
                    ExplorerLeftPanel.Visibility = Visibility.Visible;
                    break;
                case PanelType.AMS_INVESTIGATION:
                    AmsInvestigationPanel.Visibility = Visibility.Visible;
                    break;
                case PanelType.AMS_INVESTIGATION_DETAIL:
                    AmsInvestigationDetailPanel.Visibility = Visibility.Visible;
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
                _renderingService.CreateErrorGraph(ActiveCalculationsDataGrid);
            }
            catch(WrongConfigurationException ex)
            {
                MessageBox.Show(ex.Message);
                OpenConfigurationWindow();
            }
        }

        private void ActiveCalculationsDataGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            DataGrid dataGrid = sender as DataGrid;
            DataGridRow row = findParentOfType<DataGridRow>(e.OriginalSource as DependencyObject);

            if (dataGrid != null && row != null && dataGrid.SelectedItems.Contains(row.DataContext))
            {
                //the row DataContext is the selected item
                dataGrid.SelectedItems.Remove(row.DataContext);
                //mark event as handled so that datagrid does not
                //just select it again on the current click.
                e.Handled = true;
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
                ret = parent as T ?? findParentOfType<T>(parent);
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
            var configWindowResult = OpenConfigurationWindowOperation.Operate(MetricsConfig.GetInstance());

            MetricsUsedTB.Text = MetricsConfig.GetInstance().ToString();

            if (currentlyDisplayedPanelType == PanelType.AMS_INVESTIGATION_DETAIL && configWindowResult.HasChanged)
            {
                CompareWithLibrary(@"libFiles\lib-proton", LibStructureType.DIRECTORY_SEPARATED);
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
            _buttonService.ExportJsonBtn_Click(sender, e, ExecutionDetailList, executionDetailSelectedIdx);
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
                            execution.ComputeError(outputFileContent, MetricsConfig.GetInstance()); // If error in computeError, this was changed recently
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
           _heatMapService.DrawHeatmapBtn_Click(sender, e, ExecutionDetailList, executionDetailSelectedIdx);
        }

        private void ExportListAsCsvBtn_Click(object sender, RoutedEventArgs e)
        {
            ExportListAsCsvOperation.Operate(AmsExecutionList);
        }
    }
}
