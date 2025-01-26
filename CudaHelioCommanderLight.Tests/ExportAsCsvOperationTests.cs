using NUnit.Framework;
using NSubstitute;
using System;
using System.Windows; // Include if needed for MessageBoxImage
using CudaHelioCommanderLight.Operations; // Adjust namespace as necessary
using CudaHelioCommanderLight.Interfaces; // Adjust namespace as necessary
using static CudaHelioCommanderLight.HeatMapGraph;

[TestFixture]
public class ExportAsCsvOperationTests
{
    private IFileWriter _mockFileWriter;
    private IDialogService _mockDialogService;
    private HeatPoint[,] _sampleHeatPoints;

    [SetUp]
    public void SetUp()
    {
        // Initialize mocks
        _mockFileWriter = Substitute.For<IFileWriter>();
        _mockDialogService = Substitute.For<IDialogService>();

        // Prepare some sample data to test with
        _sampleHeatPoints = new HeatPoint[,]
        {
            { new HeatPoint (0,0,1.0 ), new HeatPoint (1, 1, 2.0) }
        };

        // Set up a default behavior for the dialog service
        _mockDialogService.SaveFileDialog(out Arg.Any<string>(), Arg.Any<string>())
            .Returns(x =>
            {
                x[0] = "test.csv"; // Simulate a file path return
                return true;       // Simulate user clicked 'Save'
            });
    }

    [Test]
    public void Operate_WhenSaveFileDialogIsCancelled_ShouldNotWriteToFile()
    {
        // Arrange
        _mockDialogService.SaveFileDialog(out Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        // Act
        ExportAsCsvOperation.Operate(_sampleHeatPoints, _mockFileWriter, _mockDialogService);

        // Assert
        _mockFileWriter.DidNotReceive().WriteToFile(Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public void Operate_WhenSaveFileDialogIsConfirmed_ShouldWriteCorrectContentToFile()
    {
        // Arrange
        string expectedCsvContent = "0,0,1\r\n1,1,2\r\n";

        // Act
        ExportAsCsvOperation.Operate(_sampleHeatPoints, _mockFileWriter, _mockDialogService);

        // Assert
        _mockFileWriter.Received(1).WriteToFile("test.csv", Arg.Is<string>(s => s == expectedCsvContent));
    }

    [Test]
    public void Operate_WhenIOExceptionOccurs_ShouldShowErrorMessage()
    {
        // Arrange
        _mockFileWriter.When(x => x.WriteToFile(Arg.Any<string>(), Arg.Any<string>()))
                       .Do(x => throw new IOException("Simulated failure"));

        // Act
        ExportAsCsvOperation.Operate(_sampleHeatPoints, _mockFileWriter, _mockDialogService);

        // Assert
        _mockDialogService.Received(1).ShowMessage(Arg.Is<string>(m => m.Contains("error")),
                                                   Arg.Any<string>(), MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
