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

        MiraiVn Mirai;
        DebugWindow debugWindow;

        public struct fileContent
        {
            public string dir { get; set; }
            public string path { get; set; }
        }

        List <fileContent> BgContent = new List<fileContent>();

        List<fileContent> SpriteContent = new List<fileContent>();

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
            OpenFileDialog FileLocation = new OpenFileDialog();
           
            FileLocation.ShowDialog();

            if (FileLocation.FileName.CompareTo("") != 0)
            {
                ProjectFile = FileLocation.FileName;
                RootDir = ProjectFile.Substring(0, ProjectFile.LastIndexOf("\\"));

                ShowDir(RootDir);

                Mirai = new MiraiVn(ProjectFile, BgContent, SpriteContent);
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
                item.KeyDown += Folder_Expanded;

                //Separate file 
                DirectoryInfo dir = new DirectoryInfo(drive);
                FileInfo[] Files = dir.GetFiles();

                if(Path.GetFileName(drive) == "Bg")
                {
                    foreach(var file in Files)
                    {
                        BgContent.Add(new fileContent { dir = Path.GetFileName(drive), path = file.FullName });
                    }
                }

                else if(Path.GetFileName(drive) == "Sprite")
                {
                    foreach(var file in Files)
                    {
                        SpriteContent.Add(new fileContent { dir = Path.GetFileName(drive), path = file.FullName });
                    }
                }
              
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
            debugWindow = new DebugWindow(Mirai, ProjectFile, BgContent, SpriteContent);
            debugWindow.Show();        
        }

        private void StopDebug_Click(object sender, RoutedEventArgs e)
        {
            
        }       
    }
}
