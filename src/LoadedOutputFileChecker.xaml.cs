using CudaHelioCommanderLight.Enums;
using CudaHelioCommanderLight.Helpers;
using CudaHelioCommanderLight.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CudaHelioCommanderLight
{
    /// <summary>
    /// Interaction logic for LoadedOutputFileChecker.xaml
    /// </summary>
    public partial class LoadedOutputFileChecker : Window
    {
        private ObservableCollection<ExecutionRow> executionRows;
        public OutputFileContent outputFileContent { get; set; }

        private List<Grid> columnSelectorSections;
        private List<ComboBox> columnSelectorComboBoxes;
        private List<TextBlock> columnSelectorTextBlocks;
        private List<OutputFileColumnType> supportedColumns;
        private bool currentlyChangingColumns = false;

        private bool isSpectraDivided;

        public LoadedOutputFileChecker(OutputFileContent outputFileContent)
        {
            InitializeComponent();
            this.outputFileContent = outputFileContent;
            this.isSpectraDivided = false;

            InitializeSections();

            UpdateList();
            UpdateColumnSelectorSections();
            InitializeDropDownLists();
        }

        private void UpdateColumnSelectorSections()
        {
            string[] columns = outputFileContent.FirstLine.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < 6; i++)
            {
                if (i + 1 <= outputFileContent.NumberOfColumns)
                {
                    columnSelectorSections[i].Visibility = Visibility.Visible;

                    if (i + 1 <= columns.Length)
                    {
                        columnSelectorTextBlocks[i].Text = columns[i];
                    }
                    else
                    {
                        columnSelectorTextBlocks[i].Text = "Error parse";
                    }
                }
                else
                {
                    columnSelectorSections[i].Visibility = Visibility.Hidden;
                }
            }
        }

        private void UpdateList()
        {
            executionRows = new ObservableCollection<ExecutionRow>();

            if (outputFileContent.TKinList != null)
            {
                for (int i = 0; i < outputFileContent.TKinList.Count; i++)
                {
                    if (executionRows.Count > i)
                    {
                        executionRows[i].TKin = outputFileContent.TKinList[i];
                    }
                    else
                    {
                        executionRows.Add(new ExecutionRow() { TKin = outputFileContent.TKinList[i] });
                    }
                }
            }

            if (outputFileContent.Spe1e3NList != null)
            {
                for (int i = 0; i < outputFileContent.Spe1e3NList.Count; i++)
                {
                    if (executionRows.Count > i)
                    {
                        executionRows[i].Count = outputFileContent.Spe1e3NList[i];
                    }
                    else
                    {
                        executionRows.Add(new ExecutionRow() { Count = outputFileContent.Spe1e3NList[i] });
                    }
                }
            }

            if (outputFileContent.Spe1e3List != null)
            {
                for (int i = 0; i < outputFileContent.Spe1e3List.Count; i++)
                {
                    if (executionRows.Count > i)
                    {
                        executionRows[i].Spectra = outputFileContent.Spe1e3List[i];
                    }
                    else
                    {
                        executionRows.Add(new ExecutionRow() { Spectra = outputFileContent.Spe1e3List[i] });
                    }
                }
            }

            if (outputFileContent.StdDevList != null)
            {
                for (int i = 0; i < outputFileContent.StdDevList.Count; i++)
                {
                    if (executionRows.Count > i)
                    {
                        executionRows[i].StandardDeviation = outputFileContent.StdDevList[i];
                    }
                    else
                    {
                        executionRows.Add(new ExecutionRow() { StandardDeviation = outputFileContent.StdDevList[i] });
                    }
                }
            }

            if (outputFileContent.WHLISList != null)
            {
                for (int i = 0; i < outputFileContent.WHLISList.Count; i++)
                {
                    if (executionRows.Count > i)
                    {
                        executionRows[i].WHLIS = outputFileContent.WHLISList[i];
                    }
                    else
                    {
                        executionRows.Add(new ExecutionRow() { WHLIS = outputFileContent.WHLISList[i] });
                    }
                }
            }

            if (outputFileContent.OtherList != null)
            {
                for (int i = 0; i < outputFileContent.OtherList.Count; i++)
                {
                    if (executionRows.Count > i)
                    {
                        executionRows[i].Other = outputFileContent.OtherList[i];
                    }
                    else
                    {
                        executionRows.Add(new ExecutionRow() { Other = outputFileContent.OtherList[i] });
                    }
                }
            }

            ExecutionCheckDataGrid.ItemsSource = executionRows;
            ExecutionCheckDataGrid.Items.Refresh();

            SetupDivideSpectraCb();
        }

        private void SetupDivideSpectraCb()
        {
            if (outputFileContent.Spe1e3List.Count == 0 || outputFileContent.Spe1e3NList.Count == 0)
            {
                DivideSpectraCb.IsEnabled = false;
            }
            else
            {
                DivideSpectraCb.IsEnabled = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DivideSpectraCb_Checked(object sender, RoutedEventArgs e)
        {
            if (outputFileContent.Spe1e3List.Count == 0 || outputFileContent.Spe1e3NList.Count == 0)
            {
                return;
            }

            if (!isSpectraDivided && DivideSpectraCb.IsChecked == true)
            {
                for (int i = 0; i < outputFileContent.Spe1e3List.Count; i++)
                {
                    outputFileContent.Spe1e3List[i] = outputFileContent.Spe1e3List[i] / outputFileContent.Spe1e3NList[i];
                }
                isSpectraDivided = true;

                UpdateList();
            }
            else if (isSpectraDivided && DivideSpectraCb.IsChecked == false)
            {
                for (int i = 0; i < outputFileContent.Spe1e3List.Count; i++)
                {
                    outputFileContent.Spe1e3List[i] = outputFileContent.Spe1e3List[i] * outputFileContent.Spe1e3NList[i];
                }
                isSpectraDivided = false;

                UpdateList();
            }
        }

        private void ColumnSelectorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Prevent double changing columns by UI event call
            if (currentlyChangingColumns)
            {
                return;
            }

            ComboBox comboBox = sender as ComboBox;

            if (comboBox == null)
            {
                return;
            }

            bool parseSuccess = Int32.TryParse(comboBox.Tag.ToString(), out int colIdx);

            if (!parseSuccess)
            {
                Console.WriteLine("Error parsing combobox tag");
                return;
            }

            OutputFileColumnType oldType = (OutputFileColumnType)e.RemovedItems[e.RemovedItems.Count - 1];
            OutputFileColumnType columnType = (OutputFileColumnType)comboBox.SelectedItem;

            foreach (ComboBox combo in columnSelectorComboBoxes.ToList())
            {
                if (combo != comboBox && combo.SelectedItem != null && (OutputFileColumnType)combo.SelectedItem == columnType)
                {
                    currentlyChangingColumns = true;
                    combo.SelectedIndex = supportedColumns.IndexOf(oldType); // will trigger this method
                }
            }

            outputFileContent.SwapLists(oldType, columnType);
            currentlyChangingColumns = false;

            UpdateList();
        }

        private void InitializeDropDownLists()
        {
            supportedColumns = Enum.GetValues(typeof(OutputFileColumnType))
                .Cast<OutputFileColumnType>()
                .ToList();

            for (int idx = 0; idx < columnSelectorSections.Count; idx++)
            {
                Grid colGrid = columnSelectorSections[idx];
                ComboBox comboBox = colGrid.Children.OfType<ComboBox>().FirstOrDefault();
                columnSelectorComboBoxes.Add(comboBox);
                comboBox.ItemsSource = supportedColumns;

                if (idx > outputFileContent.NumberOfColumns - 1)
                {
                    comboBox.SelectedIndex = -1;
                }
                else
                {
                    MainHelper.TryConvertToDouble(columnSelectorTextBlocks[idx].Text, out double firstValue);
                    var columnType = outputFileContent.GetColumnTypeByFirstValue(firstValue);
                    comboBox.SelectedIndex = supportedColumns.IndexOf(columnType);

                    //comboBox.SelectedIndex = idx;
                }
                comboBox.SelectionChanged += new SelectionChangedEventHandler(ColumnSelectorComboBox_SelectionChanged);
            }
        }

        private void InitializeSections()
        {

            columnSelectorComboBoxes = new List<ComboBox>();
            columnSelectorTextBlocks = new List<TextBlock>()
            {
                Col1TextBlock,
                Col2TextBlock,
                Col3TextBlock,
                Col4TextBlock,
                Col5TextBlock,
                Col6TextBlock,
            };

            columnSelectorSections = new List<Grid>()
            {
                Col1Grid,
                Col2Grid,
                Col3Grid,
                Col4Grid,
                Col5Grid,
                Col6Grid,
            };
        }
    }
}
