using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.IO;
using Path = System.IO.Path;

namespace mirai_engine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string ProjectFile;
        string RootDir;

        //hide window title bar
        #region
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }       

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        private void OpenProject_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog storyFileLocation = new OpenFileDialog();
           
            storyFileLocation.ShowDialog();

            if (storyFileLocation.FileName.CompareTo("") != 0)
            {
                ProjectFile = storyFileLocation.FileName;
                RootDir = ProjectFile.Substring(0, ProjectFile.LastIndexOf("\\"));

                ShowDir(RootDir);               
            }
        }
        
        private void ShowDir(string path)
        {
            foreach (var drive in Directory.GetDirectories(path))
            {
                var item = new TreeViewItem()
                {
                    // Set the header
                    Header = Path.GetFileName(drive),
                    // And the full path
                    Tag = drive
                };

                // Add a dummy item
                item.Items.Add(null);

                // Listen out for item being expanded
                item.Expanded += Folder_Expanded;
     
                FolderView.Items.Add(item);
            }
        }

        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            #region Initial Checks

            var item = (TreeViewItem)sender;

            // If the item only contains the dummy data
            if (item.Items.Count != 1 || item.Items[0] != null)
                return;

            item.Items.Clear();

            var fullPath = (string)item.Tag;

            #endregion

            #region Get Folders

            var directories = new List<string>();

            // Try and get directories from the folder
            // ignoring any issues doing so
            try
            {
                var dirs = Directory.GetDirectories(fullPath);

                if (dirs.Length > 0)
                    directories.AddRange(dirs);
            }
            catch { }

            directories.ForEach(directoryPath =>
            {
                var subItem = new TreeViewItem()
                {
                    Header = GetFileFolderName(directoryPath),

                    Tag = directoryPath
                };

                subItem.Items.Add(null);

                // Handle expanding
                subItem.Expanded += Folder_Expanded;

                item.Items.Add(subItem);
            });

            #endregion

            #region Get Files

            var files = new List<string>();

            // Try and get files from the folder
            // ignoring any issues doing so
            try
            {
                var fs = Directory.GetFiles(fullPath);

                if (fs.Length > 0)
                    files.AddRange(fs);
            }
            catch { }

            files.ForEach(filePath =>
            {
                var subItem = new TreeViewItem()
                {
                    Header = GetFileFolderName(filePath),
                    Tag = filePath
                };

                item.Items.Add(subItem);
            });

            #endregion
        }

        #region Helpers

        /// <summary>
        /// Find the file or folder name from a full path
        /// </summary>
        /// <param name="path">The full path</param>
        /// <returns></returns>
        public static string GetFileFolderName(string path)
        {
            // If we have no path, return empty
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            // Make all slashes back slashes
            var normalizedPath = path.Replace('/', '\\');

            // Find the last backslash in the path
            var lastIndex = normalizedPath.LastIndexOf('\\');

            // If we don't find a backslash, return the path itself
            if (lastIndex <= 0)
                return path;

            // Return the name after the last back slash
            return path.Substring(lastIndex + 1);
        }

        #endregion


        private void Degug_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StopDebug_Click(object sender, RoutedEventArgs e)
        {

        }       
    }
}
