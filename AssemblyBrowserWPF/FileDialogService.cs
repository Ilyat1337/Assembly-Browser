using Microsoft.Win32;
using System.Windows;

namespace AssemblyBrowserWPF
{

    public class FileDialogService
    {
        private OpenFileDialog openFileDialog;

        public string FilePath { get; set; }

        public FileDialogService(string fileFilter)
        {
            openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = fileFilter;
        }

        public bool OpenFileDialog()
        {
            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                return true;
            }
            return false;
        }

        public void ShowMessage(string message, string caption)
        {
            MessageBox.Show(message, caption);
        }

        public void ShowErrorMessage(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

