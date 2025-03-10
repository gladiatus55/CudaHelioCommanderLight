﻿using CudaHelioCommanderLight.Config;
using CudaHelioCommanderLight.Enums;
using CudaHelioCommanderLight.Helpers;
using CudaHelioCommanderLight.Interfaces;
using CudaHelioCommanderLight.Models;
using CudaHelioCommanderLight.Operations;
using NSubstitute;
using System.Windows;

[TestFixture]
public class CompareLibraryOperationTests
{
    private string _testDir;
    private IDialogService _dialogService;
    private IMainHelper _mainHelper;
    private CompareLibraryOperation _operation;

    [SetUp]
    public void Setup()
    {
        _testDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(_testDir);

        _dialogService = Substitute.For<IDialogService>();
        _mainHelper = Substitute.For<IMainHelper>();
        _operation = new CompareLibraryOperation(_dialogService, _mainHelper);
    }

    [TearDown]
    public void TearDown()
    {
        Directory.Delete(_testDir, true);
    }

    [Test]
    public void Operate_DirectorySeparated_CorrectlyProcessesDirectories()
    {
        CreateTestStructure(new[] { "V100K03", "V200K05" }); // Removed "InvalidDir"

        var amsExecution = new AmsExecution
        {
            FileName = "test_ams_execution.dat",
            FilePath = Path.Combine(_testDir, "test_ams_execution.dat"),
            Spe1e3 = Enumerable.Range(10, 31).Select(x => (double)x).ToList(),
            TKin = Enumerable.Range(0, 31).Select(x => Math.Round(x * 0.1, 1)).ToList()
        };

        var model = new CompareLibraryModel
        {
            LibPath = _testDir,
            AmsExecution = amsExecution
        };

        var metricsConfig = MetricsConfig.GetInstance(_mainHelper);
        metricsConfig.ErrorFromGev = 0.0;
        metricsConfig.ErrorToGev = 3.0;

        // Mock data includes all TKin values and a valid FilePath
        _mainHelper.ExtractOutputDataFile(Arg.Any<string>(), out Arg.Any<OutputFileContent>())
            .Returns(x =>
            {
                x[1] = new OutputFileContent
                {
                    FilePath = Path.Combine(_testDir, "output_1e3bin.dat"),
                    TKinList = Enumerable.Range(0, 31).Select(x => Math.Round(x * 0.1, 1)).ToList(),
                    Spe1e3List = Enumerable.Range(10, 31).Select(x => (double)x).ToList(),
                    Spe1e3NList = Enumerable.Repeat(1.0, 31).ToList()
                };
                return true;
            });

        var result = _operation.Operate(model, LibStructureType.DIRECTORY_SEPARATED);
        Assert.That(result, Has.Count.EqualTo(2));
    }



    [Test]
    public void Operate_InvalidDataFile_ShowsErrorMessage()
    {
        File.WriteAllText(Path.Combine(_testDir, "test.dat"), "invalid data");

        var model = new CompareLibraryModel
        {
            LibPath = _testDir,
            AmsExecution = new AmsExecution()
            {
                Spe1e3 = new List<double> { }, // Empty list for invalid case testing.
                TKin = new List<double> { }
            }
        };

        _mainHelper.ExtractOutputDataFile(Arg.Any<string>(), out Arg.Any<OutputFileContent>())
            .Returns(false);

        var result = _operation.Operate(model, LibStructureType.FILES_SOLARPROP_LIB);

        Assert.That(result, Is.Empty);
        _dialogService.Received(1).ShowMessage(
            "Cannot read data values from the input file.",
            "Error",
            MessageBoxButton.OK,
            MessageBoxImage.Error
        );
    }

    private void CreateTestStructure(IEnumerable<string> directories)
    {
        foreach (var dir in directories)
        {
            var path = Path.Combine(_testDir, dir);
            Directory.CreateDirectory(path);

            // Write valid data to files with proper paths
            File.WriteAllText(Path.Combine(path, "output_1e3bin.dat"),
                "TKin: 0.0 0.1 0.2 ... 3.0\nSpe1e3: ...\nSpe1e3N: ...");
        }
    }



}
