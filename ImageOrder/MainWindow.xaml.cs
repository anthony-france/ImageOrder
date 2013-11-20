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
        OutputFileList exportFiles = new OutputFileList();

        private ListBoxItem _dragged;

        public MainWindow()
        {
            InitializeComponent( );
            lbFiles.ItemsSource = files;
            lbExport.ItemsSource = exportFiles;
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
                string[] fileList = Directory.GetFiles(dialog.SelectedPath, "*.jpg");
                foreach (string file in fileList)
                {
                    //files.Add(new ListItemFiles() { fileName = new FileInfo(file).Name, Path = new FileInfo(file).Directory.FullName, nameAndPath = file });
                    files.Add(new ListItemFiles() { File = new FileInfo(file), fullPath = file });
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
            while (exportFiles.Count > 0)
            {
                exportFiles.Remove(exportFiles.First());
            }
        }

        private void btnAddAll_Click(object sender, RoutedEventArgs e)
        {
            for (var index = 0; index < files.Count; index++)
            {
                ListItemFiles selected = files[index];
                exportFiles.Add(selected);
            }
        }

        private void lbFiles_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_dragged != null)
                return;

            UIElement element = lbFiles.InputHitTest(e.GetPosition(lbFiles)) as UIElement;

            while (element != null)
            {
                if (element is ListBoxItem)
                {
                    _dragged = (ListBoxItem)element;
                    break;
                }
                element = VisualTreeHelper.GetParent(element) as UIElement;
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragged == null)
                return;
            if (e.LeftButton == MouseButtonState.Released)
            {
                _dragged = null;
                return;
            }

            DataObject obj = new DataObject(DataFormats.Text, _dragged.ToString());
            DragDrop.DoDragDrop(_dragged, obj, DragDropEffects.All);
        }
        private void lbExport_DragEnter(object sender, DragEventArgs e)
        {
            if (_dragged == null || e.Data.GetDataPresent(DataFormats.Text, true) == false)
                e.Effects = DragDropEffects.None;
            else
                e.Effects = DragDropEffects.All;
        }
        private void lbExport_Drop(object sender, DragEventArgs e)
        {
            try
            {
                exportFiles.Add(_dragged.Content as ListItemFiles);
            }
            catch
            {
            }
        }

        private void lbFiles_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try 
            {
                exportFiles.Add(files[lbFiles.SelectedIndex]);
            }
            catch
            {
            }
        }

        private void lbExport_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                exportFiles.Remove(exportFiles[lbExport.SelectedIndex]);
            }
            catch
            {
            }
        }

        private void MenuItem_Click_Remove(object sender, RoutedEventArgs e)
        {
            try
            {
                exportFiles.Remove(exportFiles[lbExport.SelectedIndex]);
            }
            catch
            {
            }
        }

        private void MenuItem_Click_Weave(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItem = exportFiles[lbExport.SelectedIndex];
                exportFiles.Remove(exportFiles[lbExport.SelectedIndex]);

                for (var index = 1; index < exportFiles.Count; index = index + 2)
                {
                    exportFiles.Insert(index, selectedItem);
                }
            }
            catch
            { }
        }

        private void MenuItem_Click_MoveTop(object sender, RoutedEventArgs e)
        {
            try
            {
                ListItemFiles selectedItem = exportFiles[lbExport.SelectedIndex];
                exportFiles.Remove(selectedItem);
                exportFiles.Insert(0, selectedItem);
            }
            catch { }
        }

        private void MenuItem_Click_MoveBottom(object sender, RoutedEventArgs e)
        {
            try
            {
                ListItemFiles selectedItem = exportFiles[lbExport.SelectedIndex];
                exportFiles.Remove(selectedItem);
                exportFiles.Add(selectedItem);
            }
            catch { }
        }

        private void MenuItem_Click_MoveUp(object sender, RoutedEventArgs e)
        {
            try
            {
                var pos = lbExport.SelectedIndex - 1;
                ListItemFiles selectedItem = exportFiles[lbExport.SelectedIndex];
                exportFiles.Remove(selectedItem);
                exportFiles.Insert(pos, selectedItem);
            }
            catch { }
        }

        private void MenuItem_Click_MoveDown(object sender, RoutedEventArgs e)
        {
            try
            {
                ListItemFiles selectedItem = exportFiles[lbExport.SelectedIndex];
                exportFiles.Remove(selectedItem);
                exportFiles.Insert(lbExport.SelectedIndex + 1, selectedItem);
            }
            catch { }
            
        }

        private void MenuItem_Click_Save(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && exportFiles.Count > 0)
                {
                    string savePath = dialog.SelectedPath;
                    for (var index = 0; index < exportFiles.Count; index++)
                    {
                        var file = exportFiles[index];
                        var name = string.Format("{0:000000}{1}", index, file.File.Extension);
                        var destFile = System.IO.Path.Combine(savePath, name);
                        System.IO.File.Copy(file.File.FullName, destFile, true);
                    }
                }
            }
            catch { }

        }

        private void MenuItem_Click_Add(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new System.Windows.Forms.OpenFileDialog();
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    files.Add(new ListItemFiles() { File = new FileInfo(dialog.FileName), fullPath = dialog.FileName });
                }
            }
            catch { }
        }

        private void MenuItem_Click_Insert(object sender, RoutedEventArgs e)
        {
            try
            {
                exportFiles.Add(files[lbFiles.SelectedIndex]);
            }
            catch
            {
            }
        }
    }


    public class ListItemFiles
    {
        public FileInfo File { get; set; }
        public string fullPath { get; set; }
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
