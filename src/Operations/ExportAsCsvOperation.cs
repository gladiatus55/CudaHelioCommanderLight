using CudaHelioCommanderLight.Models;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using static CudaHelioCommanderLight.HeatMapGraph;

namespace CudaHelioCommanderLight.Operations
{
    public class ExportAsCsvOperation : Operation<HeatPoint[,]>
    {
        public static new void Operate(HeatPoint[,] HeatPoints)
        {
            // Show a save file dialog for the user to select the output file
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV Files (*.csv)|*.csv";
            if (saveFileDialog.ShowDialog() == true)
            {
                // Get the selected file path
                string filePath = saveFileDialog.FileName;

                // Create a StringBuilder to store the CSV data
                StringBuilder csvData = new StringBuilder();

                // Iterate over each HeatPoint and add its data to the CSV
                foreach (HeatPoint heatPoint in HeatPoints)
                {

                    // Build the CSV row
                    string csvRow = $"{ConvertToString(heatPoint.X)},{ConvertToString(heatPoint.Y)},{ConvertToString(heatPoint.Intensity)}";

                    // Append the row to the CSV data
                    csvData.AppendLine(csvRow);
                }

                try
                {
                    // Write the CSV data to the selected file
                    File.WriteAllText(filePath, csvData.ToString());

                    MessageBox.Show("Export successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"An error occurred while exporting the CSV file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public static void Operate(IEnumerable<ErrorStructure> errors)
        {
            // Show a save file dialog for the user to select the output file
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV Files (*.csv)|*.csv";
            if (saveFileDialog.ShowDialog() == true)
            {
                // Get the selected file path
                string filePath = saveFileDialog.FileName;

                // Create a StringBuilder to store the CSV data
                StringBuilder csvData = new StringBuilder();

                // Iterate over each HeatPoint and add its data to the CSV
                foreach (ErrorStructure error in errors)
                {

                    // Build the CSV row
                    string csvRow = $"{ConvertToString(error.K0)},{ConvertToString(error.V)},{ConvertToString(error.Error)}";

                    // Append the row to the CSV data
                    csvData.AppendLine(csvRow);
                }

                try
                {
                    // Write the CSV data to the selected file
                    File.WriteAllText(filePath, csvData.ToString());

                    MessageBox.Show("Export successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"An error occurred while exporting the CSV file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private static string ConvertToString(double number)
        {
            return number.ToString().Replace(',', '.');
        }
    }
}