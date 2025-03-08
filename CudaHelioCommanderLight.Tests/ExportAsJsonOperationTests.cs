using NUnit.Framework;
using NSubstitute;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using CudaHelioCommanderLight.Operations;
using CudaHelioCommanderLight.Dtos;
using CudaHelioCommanderLight.Models;
using CudaHelioCommanderLight.Enums; // Ensure you include this

[TestFixture]
public class ExportAsJsonOperationTests
{
    private ExecutionListExportModel _sampleExecutionModel;
    private string _testFilePath;

    [SetUp]
    public void SetUp()
    {
        _testFilePath = "test.json"; // Simulated file path

        // Sample data setup - Providing required constructor arguments
        _sampleExecutionModel = new ExecutionListExportModel
        {
            FilePath = _testFilePath,
            Executions = new List<Execution>
            {
                new Execution(2.0, 1.0, 10, 0.1, MethodType.FP_1D),  // V, K0, N, dt, MethodType
                new Execution(3.0, 2.0, 20, 0.2, MethodType.BT_2D)   // V, K0, N, dt, MethodType
            }
        };
    }

    [Test]
    public void Operate_ShouldCreateJsonFileWithExpectedContent()
    {
        // Act
        ExportAsJsonOperation.Operate(_sampleExecutionModel);

        // Assert
        Assert.That(File.Exists(_testFilePath), Is.True, "JSON file was not created.");

        // Read the file and check content
        string jsonContent = File.ReadAllText(_testFilePath);
        List<ExecutionDto> deserializedData = JsonConvert.DeserializeObject<List<ExecutionDto>>(jsonContent);

        Assert.That(deserializedData, Is.Not.Null);
        Assert.That(deserializedData.Count, Is.EqualTo(_sampleExecutionModel.Executions.Count));
        Assert.That(deserializedData[0].K0, Is.EqualTo(_sampleExecutionModel.Executions[0].K0));
        Assert.That(deserializedData[0].Method, Is.EqualTo(nameof(MethodType.FP_1D))); // Correctly check enum string

        // Cleanup
        File.Delete(_testFilePath);
    }

    [Test]
    public void Operate_WhenIOExceptionOccurs_ShouldThrowException()
    {
        // Arrange
        var executionModel = new ExecutionListExportModel
        {
            FilePath = @"C:\invalid\path\test.json", // Invalid path
            Executions = new List<Execution>
        {
            new Execution(1.0, 2.0, 3.0, 4.0, MethodType.FP_1D)
        }
        };

        // Act & Assert
        var ex = Assert.Throws<IOException>(() => ExportAsJsonOperation.Operate(executionModel));

        // Ensure it is the expected exception
        Assert.That(ex, Is.InstanceOf<IOException>());
    }
}
