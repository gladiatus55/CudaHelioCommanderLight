using CudaHelioCommanderLight.Models;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;

namespace CudaHelioCommanderLight.Operations
{
    public class ExportListAsCsvOperation : Operation<ObservableCollection<AmsExecution>>
    {
        public static new void Operate(ObservableCollection<AmsExecution> exportList)
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
                foreach (var record in exportList)
                {

                    // Build the CSV row
                    string csvRow = $"{record.FileName},{record.LowestRMSError},{record.LowestRMSErrorFile},{record.MinimalMaxError},{record.MinimalMaxErrorFile}";

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
    }
}
