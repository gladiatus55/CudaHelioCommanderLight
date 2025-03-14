using CudaHelioCommanderLight.Constants;
using CudaHelioCommanderLight.Enums;
using CudaHelioCommanderLight.Helpers;
using CudaHelioCommanderLight.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

[TestFixture]
public class MainHelperTests
{
    private MainHelper _mainHelper;
    private string _testDir;

    [SetUp]
    public void Setup()
    {
        _mainHelper = new MainHelper();
        _testDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(_testDir);
    }

    [TearDown]
    public void TearDown()
    {
        Directory.Delete(_testDir, true);
    }

    [Test]
    public void ExtractMultipleOfflineStatus_ValidFiles_ReturnsAmsExecutionDetail()
    {
        CreateTestFiles(new[] { "file1.txt", "file2.txt" });
        File.WriteAllText(Path.Combine(_testDir, "file1.txt"), "1.0 2.0");
        File.WriteAllText(Path.Combine(_testDir, "file2.txt"), "");

        var result = _mainHelper.ExtractMultipleOfflineStatus(new[]
        {
        Path.Combine(_testDir, "file1.txt"),
        Path.Combine(_testDir, "file2.txt")
    });

        Assert.That(result.AmsExecutions.Count, Is.EqualTo(1));
    }

    [Test]
    public void ExtractOutputDataFile_ValidFile_ReturnsTrue()
    {
        var filePath = Path.Combine(_testDir, "output.txt");
        File.WriteAllText(filePath, "1.0 2.0 3.0 4.0");

        var success = _mainHelper.ExtractOutputDataFile(filePath, out OutputFileContent content);

        Assert.IsTrue(success);
        Assert.That(content.TKinList.Count, Is.EqualTo(1));
    }

    [Test]
    public void ExtractForceFieldOutputDataFile_InvalidFile_ReturnsFalse()
    {
        var result = _mainHelper.ExtractForceFieldOutputDataFile("invalid_path.txt", out _);

        Assert.IsFalse(result);
    }

    [Test]
    public void TryConvertToDouble_ValidString_ReturnsTrue()
    {
        // Save the original culture
        var originalCulture = CultureInfo.CurrentCulture;

        try
        {
            // Force a culture that uses "," as decimal separator (e.g., French)
            CultureInfo.CurrentCulture = new CultureInfo("fr-FR");

            bool success = _mainHelper.TryConvertToDouble("123,45", out double result);

            Assert.IsTrue(success, "Conversion should succeed");
            Assert.That(result, Is.EqualTo(123.45).Within(0.001), "Result should be 123.45");
        }
        finally
        {
            // Restore the original culture
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Test]
    public void TryConvertToDecimal_InvalidString_ReturnsFalse()
    {
        var success = _mainHelper.TryConvertToDecimal("invalid", out _);

        Assert.IsFalse(success);
    }

    private void CreateTestDirectories(IEnumerable<string> dirs)
    {
        foreach (var dir in dirs)
        {
            Directory.CreateDirectory(Path.Combine(_testDir, dir));
        }
    }

    private void CreateTestFiles(IEnumerable<string> files)
    {
        foreach (var file in files)
        {
            File.Create(Path.Combine(_testDir, file)).Close();
        }
    }
}
