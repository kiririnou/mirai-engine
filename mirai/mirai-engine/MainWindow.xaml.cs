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
using FolderBrowser;

using Path = System.IO.Path;

namespace mirai_engine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string ProjectFile;
        private string RootDir;
        private int ProjectLineEnd;
        private List<eventHierarchy> ProjectLines;

        private MiraiVn Mirai;
        private DebugWindow debugWindow;
        private MiraiBuild miraiBuild; 

        public struct fileData
        {
            public string dir { get; set; }
            public string path { get; set; }
        }

        private List <fileData> BgData = new List<fileData>();
        private List<fileData> SpriteData = new List<fileData>();
        private List<fileData> MusicData = new List<fileData>();

        public struct eventHierarchy
        {
            public string sceneName { get; set; }
            public List<eventHierarchy> EventContetn { get; set; }

            public int sceneCount { get; set; }
        }

        List<eventHierarchy> EventHierarchy = new List<eventHierarchy>();

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

                Mirai = new MiraiVn(ProjectFile, BgData, SpriteData, MusicData);
                miraiBuild = new MiraiBuild(ProjectFile);

                ShowDir(RootDir);

                SaveAsMenu.IsEnabled = true;
            }
        }
        
        private void ShowDir(string path)
        {
            ProjectLines = Mirai.GetProjectLines(out ProjectLineEnd);  
 
            foreach (var line in ProjectLines)
            { 
                var item = new TreeViewItem()
                {
                    Header = line.sceneName,

                    Tag = line.sceneCount
                };

                // Add a dummy item
                item.Items.Add(null);

                // Listen out for item being expanded
                item.Expanded += Folder_Expanded;

                FolderView.Items.Add(item);
            }

            //get resources
            foreach (var drive in Directory.GetDirectories(path))
            {
                DirectoryInfo dir = new DirectoryInfo(drive);
                FileInfo[] Files = dir.GetFiles();

                if (Path.GetFileName(drive) == "Bg")
                {
                    foreach (var file in Files)
                    {
                        BgData.Add(new fileData { dir = Path.GetFileName(drive), path = file.FullName });
                    }
                }
                else if (Path.GetFileName(drive) == "Sprite")
                {
                    foreach (var file in Files)
                    {
                        SpriteData.Add(new fileData { dir = Path.GetFileName(drive), path = file.FullName });
                    }
                }
                else if (Path.GetFileName(drive) == "Music")
                {
                    foreach (var file in Files)
                    {
                        MusicData.Add(new fileData { dir = Path.GetFileName(drive), path = file.FullName });
                    }
                }
            }           
        }

        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            var item = (TreeViewItem)sender;

            // If the item only contains the dummy data
            if (item.Items.Count != 1 || item.Items[0] != null)
                return;

            item.Items.Clear();

            int sceneCount = Convert.ToInt32(item.Tag);

            eventHierarchy sceneData = ProjectLines[sceneCount];

            int LineCount = 0;
            foreach(var line in sceneData.EventContetn)
            { 
                var subItem = new TreeViewItem()
                {
                    Header = line.sceneName,
                    Tag = line.sceneCount
                };                             

                item.Items.Add(subItem);

                subItem.PreviewMouseLeftButtonDown += MouseLeftButtonDown;

                LineCount++;
            }           
        }

        private void MouseLeftButtonDown(object sender,RoutedEventArgs e)
        {
            var item = (TreeViewItem)sender;           

            item.Items.Clear();

            oldScriptLine = Convert.ToInt32(item.Tag);
            oldScriptItem = item;

            ItemContextTextBox.Text = item.Header.ToString();
          

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
            debugWindow = new DebugWindow(Mirai, ProjectFile, BgData, SpriteData, MusicData);
            debugWindow.Show();        
        }

        private void StopDebug_Click(object sender, RoutedEventArgs e)
        {
            debugWindow.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizateButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }
        private void NewProject_Click(object sender, RoutedEventArgs e)
        {   
            RootDir = miraiBuild.GetDirPath();

            Directory.CreateDirectory(RootDir + "\\Bg");
            Directory.CreateDirectory(RootDir + "\\Sprite");
            Directory.CreateDirectory(RootDir + "\\Music");

            using (StreamWriter writer = new StreamWriter(RootDir + "\\Scenario.txt"))
            {
                writer.WriteLine("new-scene");
                writer.WriteLine("end-scene");
                writer.WriteLine("...");
                writer.Close();
            }
        }

        private void SaveMenu_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void SaveAsMenu_Click(object sender, RoutedEventArgs e)
        {
            miraiBuild.StartBuild(RootDir);            
        }

        //------------------------------------------novel interface---------------------------------------------------    

        private TreeViewItem oldScriptItem;
        private int oldScriptLine;

        private void NewScene_Click(object sender, RoutedEventArgs e)
        {
            using(StreamWriter writer = new StreamWriter(ProjectFile))
            {
                int count=0;
               for(int i=0;i<=ProjectLines.Count;i++)
                {
                    if (count == ProjectLineEnd)
                    {
                        writer.WriteLine("new-scene");
                        writer.WriteLine("end");
                    }
                    //try to fix
                    writer.WriteLine(ProjectLines[count].sceneName);
                    count++;
                }

                writer.Close();
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (oldScriptItem.Header != ItemContextTextBox.Text)
            {
                string[] lines = File.ReadAllLines(ProjectFile);
                using (StreamWriter writer = new StreamWriter(ProjectFile))
                {
                    for (int currentLine = 1; currentLine <= lines.Length; ++currentLine)
                    {
                        if (currentLine == oldScriptLine)
                        {
                            writer.WriteLine(ItemContextTextBox.Text);
                        }
                        else if(currentLine != oldScriptLine+1)
                        {
                            writer.WriteLine(lines[currentLine - 1]);
                        }
                    }
                }

                oldScriptItem.Header = ItemContextTextBox.Text;
            }
        }
    }
}
