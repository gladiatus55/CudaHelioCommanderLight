﻿using NUnit.Framework;
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

[TestFixture]
[Apartment(ApartmentState.STA)]
public class MainWindowTests
{
    private MainWindow _mainWindow;
    private IMainHelper _mainHelper;
    private IDialogService _dialogService;
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
        _buttonService = Substitute.For<ButtonService>(_dialogService);
        _renderingService = Substitute.For<RenderingService>(_mainHelper);
        _heatMapService = Substitute.For<HeatMapService>();
        _compareService = Substitute.For<CompareService>();
        _fileWriter = Substitute.For<IFileWriter>();
        _metricsConfig = Substitute.For<IMetricsConfig>();
        _compareLibraryOperation = Substitute.For<CompareLibraryOperation>(_dialogService,_mainHelper,_metricsConfig);
        _mainWindow = new MainWindow(
            _mainHelper, _dialogService, _buttonService, _renderingService,
            _heatMapService, _compareService, _fileWriter, _compareLibraryOperation, _metricsConfig,true
        );
        _mainWindow.amsErrorsListBox = new ListBox();
        _dialogService.SaveFileDialog(out Arg.Any<string>(), Arg.Any<string>())
            .Returns(x =>
            {
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

    [Test]
    public void CompareWithLibrary_WhenLibraryNotFound_ShowsErrorMessage()
    {
        // Arrange
        string invalidPath = @"C:\InvalidPath";

        // Act
        _mainWindow.CompareWithLibrary(invalidPath, LibStructureType.DIRECTORY_SEPARATED);

        // Assert
        _dialogService.Received(1).ShowMessage("Library not found", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
    }

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
        _renderingService.Received(1).RenderAmsGraph(amsExecution, new WpfPlot(), errorStructure);
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
}
