using NUnit.Framework;
using NSubstitute;
using CudaHelioCommanderLight.Interfaces;
using CudaHelioCommanderLight.Models;
using CudaHelioCommanderLight.Enums;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using CudaHelioCommanderLight.Helpers;

namespace CudaHelioCommanderLight.Tests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class LoadedOutputFileCheckerTests
    {
        private LoadedOutputFileChecker _checker;
        private IMainHelper _mockMainHelper;
        private OutputFileContent _outputFileContent;

        [SetUp]
        public void Setup()
        {
            _mockMainHelper = Substitute.For<IMainHelper>();
            _outputFileContent = new OutputFileContent
            {
                FirstLine = "TKin Spectra Count StdDev",
                NumberOfColumns = 4,
                TKinList = new List<double> { 1.0, 2.0, 3.0 },
                Spe1e3List = new List<double> { 10.0, 20.0, 30.0 },
                Spe1e3NList = new List<double> { 100, 200, 300 },
                StdDevList = new List<double> { 0.1, 0.2, 0.3 }
            };
            _checker = new LoadedOutputFileChecker(_outputFileContent, _mockMainHelper);
        }

        [Test]
        public void Constructor_InitializesCorrectly()
        {
            Assert.That(_checker.outputFileContent, Is.EqualTo(_outputFileContent));
            Assert.That(_checker.ExecutionCheckDataGrid.Items.Count, Is.EqualTo(4));
        }

        [Test]
        public void UpdateList_PopulatesExecutionRowsCorrectly()
        {
            var rows = _checker.ExecutionCheckDataGrid.ItemsSource.Cast<LoadedOutputFileChecker.ExecutionRow>().ToList();
            Assert.That(rows.Count, Is.EqualTo(3));
            Assert.That(rows[0].TKin, Is.EqualTo(1.0));
            Assert.That(rows[0].Spectra, Is.EqualTo(10.0));
            Assert.That(rows[0].Count, Is.EqualTo(100));
            Assert.That(rows[0].StandardDeviation, Is.EqualTo(0.1));
        }

        [Test]
        public void DivideSpectraCb_Checked_DividesSpectra()
        {
            _checker.DivideSpectraCb.IsChecked = true;
            _checker.DivideSpectraCb_Checked(null, null);

            var rows = _checker.ExecutionCheckDataGrid.ItemsSource.Cast<LoadedOutputFileChecker.ExecutionRow>().ToList();
            Assert.That(rows[0].Spectra, Is.EqualTo(0.1).Within(0.001));
            Assert.That(rows[1].Spectra, Is.EqualTo(0.1).Within(0.001));
            Assert.That(rows[2].Spectra, Is.EqualTo(0.1).Within(0.001));
        }
    }
}
