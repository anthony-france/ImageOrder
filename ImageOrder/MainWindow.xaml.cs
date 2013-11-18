using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageOrder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        InputFileList files = new InputFileList();
        InputFileList exportFiles = new InputFileList();

        public MainWindow()
        {
            InitializeComponent( );
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                Application.Current.Shutdown();
            }
            else
            {


                string[] filePaths = Directory.GetFiles(dialog.SelectedPath, "*.jpg");
                foreach (string filePath in filePaths)
                {
                    files.Add(new ListItemFiles() { filePath = filePath });
                }
            }
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                exportFiles.Remove(exportFiles[lbExport.SelectedIndex]);
            }
            catch
            { 
                
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                exportFiles.Add(files[lbFiles.SelectedIndex]);
            }
            catch
            {

            }
        }

        private void btnRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            for (var index = 0; index >= exportFiles.Count; index++ )
            {
                exportFiles.RemoveAt(index);
            }
            
        }

        private void btnAddAll_Click(object sender, RoutedEventArgs e)
        {
            exportFiles = files;
        }
    }

    public class ListItemFiles
    {
        public string filePath { get; set; }
    }

    public class InputFileList : ObservableCollection<ListItemFiles>
    {


    }

    public class OutputFileList : ObservableCollection<ListItemFiles>
    {

    }

    public class DoubleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Round((double)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
