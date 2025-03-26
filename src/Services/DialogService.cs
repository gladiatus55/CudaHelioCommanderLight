﻿using CudaHelioCommanderLight.Interfaces;
using System.Windows;
using Microsoft.Win32;

namespace CudaHelioCommanderLight.Services
{
    public class DialogService : IDialogService
    {
        public bool SaveFileDialog(out string filePath, string filter)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog { Filter = filter };
            bool result = saveFileDialog.ShowDialog() == true;
            filePath = result ? saveFileDialog.FileName : null;
            return result;
        }

        public void ShowMessage(string text, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            MessageBox.Show(text, caption, button, icon);
        }
    }
}
