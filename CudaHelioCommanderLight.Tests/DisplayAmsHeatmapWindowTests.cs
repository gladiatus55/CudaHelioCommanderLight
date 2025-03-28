using NUnit.Framework;
using NSubstitute;
using CudaHelioCommanderLight.Models;
using CudaHelioCommanderLight.Interfaces;
using CudaHelioCommanderLight.Operations;
using System.Collections.Generic;
using CudaHelioCommanderLight.Helpers;
using static CudaHelioCommanderLight.HeatMapGraph;
using System.Windows;

namespace CudaHelioCommanderLight.Tests
{
    [TestFixture]
    [Apartment(System.Threading.ApartmentState.STA)]
    public class DisplayAmsHeatmapWindowOperationTests
    {
        private IDisplayAmsHeatmapWindowOperation _operation;
        private IDialogService _mockDialogService;
        private IHeatMapGraphFactory _mockGraphFactory;
        private IHeatMapGraph _mockHeatMap;

        [SetUp]
        public void Setup()
        {
            _mockDialogService = Substitute.For<IDialogService>();
            _mockGraphFactory = Substitute.For<IHeatMapGraphFactory>();
            _mockHeatMap = Substitute.For<IHeatMapGraph>();

            _mockGraphFactory.Create().Returns(_mockHeatMap);

            _operation = new DisplayAmsHeatmapWindowOperation(
                _mockDialogService,
                _mockGraphFactory
            );
        }

        [Test]
        public void Operate_ValidInput_CreatesHeatmap()
        {
            // Arrange
            var model = new DisplayAmsHeatmapModel
            {
                Errors = new List<ErrorStructure>
        {
            new ErrorStructure(Substitute.For<IMainHelper>()) { K0 = 1, V = 100 },
            new ErrorStructure(Substitute.For<IMainHelper>()) { K0 = 2, V = 200 }
        },
                Tag = "RMS",
                GraphName = "Test"
            };

            // Act
            _operation.Operate(model);

            // Assert
            _mockGraphFactory.Received(1).Create();
            _mockHeatMap.Received(1).SetPoints(Arg.Any<HeatMapGraph.HeatPoint[,]>(), 2, 2);
            _mockHeatMap.Received(1).Render();
        }


        [Test]
        public void Operate_SmallXSize_ShowsErrorMessage()
        {
            // Arrange
            var model = new DisplayAmsHeatmapModel
            {
                Errors = new List<ErrorStructure>
                {
                    new ErrorStructure(Substitute.For<IMainHelper>()) { K0 = 1, V = 100 }
                },
                Tag = "RMS"
            };

            // Act
            _operation.Operate(model);

            // Assert
            _mockDialogService.Received(1).ShowMessage(
                "Cannot make map, size too small",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
        }

        [Test]
        public void Operate_SmallYSize_ShowsErrorMessage()
        {
            // Arrange
            var model = new DisplayAmsHeatmapModel
            {
                Errors = new List<ErrorStructure>
                {
                    new ErrorStructure(Substitute.For<IMainHelper>()) { K0 = 1, V = 100 },
                    new ErrorStructure(Substitute.For<IMainHelper>()) { K0 = 1, V = 200 }
                },
                Tag = "RMS"
            };

            // Act
            _operation.Operate(model);

            // Assert
            _mockDialogService.Received(1).ShowMessage(
                "Cannot make map, size too small",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
        }

        [Test]
        public void Operate_EmptyErrors_ShowsError()
        {
            // Arrange
            var model = new DisplayAmsHeatmapModel
            {
                Errors = new List<ErrorStructure>(),
                Tag = "RMS"
            };

            // Act
            _operation.Operate(model);

            // Assert
            _mockDialogService.Received(1).ShowMessage(
                "Cannot make map, size too small",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
        }
    }
}
