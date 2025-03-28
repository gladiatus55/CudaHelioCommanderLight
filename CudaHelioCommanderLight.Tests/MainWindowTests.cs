using NUnit.Framework;
using NSubstitute;
using CudaHelioCommanderLight.Interfaces;
using CudaHelioCommanderLight.Models;
using CudaHelioCommanderLight.Services;
using CudaHelioCommanderLight.Operations;
using System.Collections.ObjectModel;
using System.Windows;
using CudaHelioCommanderLight.Enums;
using CudaHelioCommanderLight.Helpers;
using CudaHelioCommanderLight.MainWindowServices;
using CudaHelioCommanderLight;
using System.Windows.Controls;
using ScottPlot;
using CudaHelioCommanderLight.Wrappers;

[TestFixture]
[Apartment(ApartmentState.STA)]
public class MainWindowTests
{
    private MainWindow _mainWindow;
    private IMainHelper _mainHelper;
    private IDialogService _dialogService;
    private IHeatMapGraphFactory _heatMapGraphFactory;
    private ButtonService _buttonService;
    private RenderingService _renderingService;
    private HeatMapService _heatMapService;
    private CompareService _compareService;
    private IFileWriter _fileWriter;
    private CompareLibraryOperation _compareLibraryOperation;
    private IMetricsConfig _metricsConfig;

[SetUp]
public void Setup()
{
    _mainHelper = Substitute.For<IMainHelper>();
    _dialogService = Substitute.For<IDialogService>();
    _heatMapGraphFactory = Substitute.For<IHeatMapGraphFactory>();
    var displayAmsHeatmapWindowOperation = Substitute.For<IDisplayAmsHeatmapWindowOperation>();

    // Initialize services with correct dependencies
    _buttonService = new ButtonService(_dialogService);
    _renderingService = Substitute.For<RenderingService>(_mainHelper, _dialogService);
    _heatMapService = Substitute.For<HeatMapService>(_dialogService, _heatMapGraphFactory, displayAmsHeatmapWindowOperation);
    _compareService = Substitute.For<CompareService>(_dialogService);
    _fileWriter = Substitute.For<IFileWriter>();
    _metricsConfig = Substitute.For<IMetricsConfig>();

    _compareLibraryOperation = new CompareLibraryOperation(
        _dialogService,
        _mainHelper,
        _metricsConfig
    );

    _mainWindow = new MainWindow(
        mainHelper: _mainHelper,
        dialogService: _dialogService,
        buttonService: _buttonService,
        renderingService: _renderingService,
        heatMapService: _heatMapService,
        compareService: _compareService,
        fileWriter: _fileWriter,
        compareLibraryOperation: _compareLibraryOperation,
        metricsConfig: _metricsConfig,
        true
    );

    _mainWindow.amsErrorsListBox = new ListBox();
    _dialogService.SaveFileDialog(out Arg.Any<string>(), Arg.Any<string>())
        .Returns(x => {
            x[0] = "test.csv";
            return true;
        });
}


    [Test]
    public void AboutUsButton_Click_CallsDialogService()
    {
        // Act
        _mainWindow.AboutUsButton_Click(null, null);

        // Assert
        _dialogService.Received(1).ShowMessage(
            "Slovak Academy of Sciences\n\nDeveloped by: Martin Nguyen, Pavol Bobik\n\nCopyright 2023",
            "About Us",
            MessageBoxButton.OK,
            MessageBoxImage.Information
        );
    }
    /*
    [Test]
    public void CompareWithLibrary_WhenLibraryNotFound_ShowsErrorMessage()
    {
        // Arrange
        string invalidPath = @"C:\InvalidPath";

        // Act
        _mainWindow.CompareWithLibrary(invalidPath, LibStructureType.DIRECTORY_SEPARATED);

        // Assert
        _dialogService.Received(1).ShowMessage(
            Arg.Is("Library not found"),
            Arg.Is("Error"),
            Arg.Is(MessageBoxButton.OK),
            Arg.Is(MessageBoxImage.Warning)
        );
    }*/


    [Test]
    public void ExportAsCsvBtn_Click_CallsExportAsCsvOperation()
    {
        // Arrange
        var testErrors = new List<ErrorStructure>
        {
            new ErrorStructure(_mainHelper) { K0 = 1.23, V = 45, Error = 0.5 },
            new ErrorStructure(_mainHelper) { K0 = 2.34, V = 67, Error = 1.2 }
        };
        _mainWindow.amsErrorsListBox.ItemsSource = testErrors;

        // Act
        _mainWindow.ExportAsCsvBtn_Click(null, null);

        // Assert
        _fileWriter.Received(1).WriteToFile(Arg.Any<string>(), Arg.Any<string>());
        _dialogService.Received(1).ShowMessage("Export successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [Test]
    public void RenderAmsGraph_CallsRenderingService()
    {
        // Arrange
        var amsExecution = new AmsExecution
        {
            TKin = new List<double> { 1.0, 2.0, 3.0 },
            Spe1e3 = new List<double> { 10, 20, 30 }
        };
        var errorStructure = new ErrorStructure(_mainHelper)
        {
            TKinList = new List<double> { 1.0 },
            Spe1e3binList = new List<double> { 50.0 },
            FilePath = "error_data/errors.txt"
        };

        // Act
        _mainWindow.RenderAmsGraph(amsExecution, errorStructure);

        // Assert
        _renderingService.Received(1).RenderAmsGraph(amsExecution, new WpfPlotWrapper(new WpfPlot()), errorStructure);
    }


    [Test]
    public void CompareAllLoadedWithLib_WhenLibraryNotFound_ShowsErrorMessage()
    {
        // Arrange
        string invalidPath = @"C:\InvalidPath";

        // Act
        _mainWindow.CompareAllLoadedWithLib(invalidPath, LibStructureType.DIRECTORY_SEPARATED);

        // Assert
        _dialogService.Received(1).ShowMessage("Library not found", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
    [Test]
    public void DrawAmsHeatmapBtn_Click_CallsHeatMapService()
    {
        // Arrange
        var button = new Button { Tag = "TestTag" };
        _mainWindow.currentDisplayedAmsInvestigation = new AmsExecution { FileName = "TestFile" };
        // Arrange
        var errorStructure = new ErrorStructure(_mainHelper)
        {
            TKinList = new List<double> { 1.0 },
            Spe1e3binList = new List<double> { 50.0 },
            FilePath = "error_data/errors.txt"
        };

        var amsExecution = new AmsExecution
        {
            TKin = new List<double> { 1.0, 2.0, 3.0 },
            Spe1e3 = new List<double> { 10, 20, 30 }
        };
        _mainWindow.amsComputedErrors = new List<ErrorStructure>();
        _mainWindow.amsComputedErrors.Add(errorStructure);
        _mainWindow.AmsExecutionList = new ObservableCollection<AmsExecution> { amsExecution };

        // Act
        _mainWindow.DrawAmsHeatmapBtn_Click(button, null);

        // Assert
        _heatMapService.Received(1).DrawAmsHeatmapBtn("TestFile", _mainWindow.amsComputedErrors, "TestTag");
    }
    /*
    [Test]
    public void CompareWithLib_Click_CallsCompareService()
    {
        // Arrange
        var button = new Button { Tag = "TestTag" };
        var expectedReturn = ("TestPath", LibStructureType.DIRECTORY_SEPARATED);
        _compareService.CompareWithLib(Arg.Any<string>(), Arg.Any<ComboBox>(), Arg.Any<ComboBox>())
            .Returns(expectedReturn);

        // Act
        _mainWindow.CompareWithLib_Click(button, null);

        // Assert
        _compareService.Received(1).CompareWithLib("TestTag", Arg.Any<ComboBox>(), Arg.Any<ComboBox>());
    }

    
    [Test]
    public void AmsErrorsListBox_SelectionChanged_CallsRenderingService()
    {
        // Arrange
        var errorStructure = new ErrorStructure(_mainHelper)
        {
            TKinList = new List<double> { 1.0 },
            Spe1e3binList = new List<double> { 50.0 },
            FilePath = "error_data/errors.txt"
        };

        var amsExecution = new AmsExecution
        {
            TKin = new List<double> { 1.0, 2.0, 3.0 },
            Spe1e3 = new List<double> { 10, 20, 30 }
        };
        _mainWindow.amsComputedErrors.Add(errorStructure);
        _mainWindow.AmsExecutionList.Add(amsExecution);

        // Act
        _mainWindow.AmsErrorsListBox_SelectionChanged(null, null);

        // Assert
        _renderingService.Received(1).AmsErrorsListBox_SelectionChanged(
            errorStructure,
            Arg.Any<IWpfPlotWrapper>(),
            Arg.Any<IWpfPlotWrapper>(),
            Arg.Any<AmsExecution>()
        );
    }
    
    [Test]
    public void CreateErrorGraphBtn_Click_CallsRenderingService()
    {
        // Act
        _mainWindow.CreateErrorGraphBtn_Click(null, null);

        // Assert
        _renderingService.Received(1).CreateErrorGraph(Arg.Any<DataGrid>());
    }

    [Test]
    public void DrawHeatmapBtn_Click_CallsHeatMapService()
    {
        // Act
        _mainWindow.DrawHeatmapBtn_Click(null, null);

        // Assert
        _heatMapService.Received(1).DrawHeatmapBtn(Arg.Any<ObservableCollection<ExecutionDetail>>(), Arg.Any<int>());
    }
    */
}
