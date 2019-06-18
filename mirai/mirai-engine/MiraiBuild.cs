using FolderBrowser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.IO;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using System.Security;
using System.Reflection;

namespace mirai_engine
{
    class MiraiBuild
    {
        private readonly string projectPath;
        public MiraiBuild(string path)
        {
            projectPath = path.Replace("\\script.txt","");
        }

        //Directory explorer
        #region 
        private string m_CdromChildPath = null;
        [DllImport("kernel32.dll")]
        public static extern long GetDriveType(string driveLetter);
        private ExtendedFolderBrowser m_ExtendedFolderBrowser = null;

        private enum VolumeTypes
        {
            Unknown,    // The drive type cannot be determined. 
            Invalid,    // The root path is invalid. For example, no volume is mounted at the path. 
            Removable,  // The disk can be removed from the drive. 
            Fixed,      // The disk cannot be removed from the drive. 
            Remote,     // The drive is a remote (network) drive. 
            CDROM,      // The drive is a CD-ROM drive. 
            RAMDisk     // The drive is a RAM disk. 
        }

        private bool IsCDDrive(string selectedPath)
        {
            if ((selectedPath != null) && (selectedPath.Trim().Length > 0))
            {
                VolumeTypes volType = (VolumeTypes)GetDriveType(selectedPath.ToLower().Substring(0, 2));
                return ((volType == VolumeTypes.CDROM) || (selectedPath.StartsWith(m_CdromChildPath)));
            }
            return false;
        }

        public string GetDirPath()
        {
            //Set the CDROM folder
            m_CdromChildPath = Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + @"\Microsoft\CD Burning\";
            m_ExtendedFolderBrowser = new ExtendedFolderBrowser();

            m_ExtendedFolderBrowser.Description = "Folder Browser";

            //Create a hanlder to a function which will check if to show the 'Make New Button' button
            ShowNewButtonHandler handler = new ShowNewButtonHandler(IsCDDrive);
            //Set the handler
            m_ExtendedFolderBrowser.SetNewFolderButtonCondition = handler;


            string dirPath = null;
            try
            {
                if (m_ExtendedFolderBrowser.ShowDialog() == DialogResult.OK)
                {
                    dirPath = m_ExtendedFolderBrowser.SelectedPath;
                }
            }
            catch (Exception)
            {
                dirPath = projectPath+"\\newVn";
            }

            return dirPath;
        }
        #endregion

        //creating file to build
        #region

        private void CreateFileToBuild()
        {
            string toPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),"")
            .Replace(@"mirai-engine\bin\Debug","")+"\\DebugData";

            DirectoryCopy(toPath,projectPath,true);
            
            DirectoryCopy(projectPath+"\\Bg",projectPath+@"\WpfApp2\bin\Debug\res\Bg",true);
            DirectoryCopy(projectPath+"\\Sprite",projectPath+@"\WpfApp2\bin\Debug\res\Sprite",true);
            DirectoryCopy(projectPath+"\\Music",projectPath+@"\WpfApp2\bin\Debug\res\Music",true);
            File.Copy(projectPath+"\\script.txt",projectPath+@"\WpfApp2\bin\Debug\res\script.txt");
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        private void RemoveBuildFile(string toPath)
        {
            DirectoryCopy(projectPath+@"\WpfApp2\bin\Debug",toPath,true);
            try
            {
                var projDir = new DirectoryInfo(projectPath + "\\WpfApp2");
                projDir.Delete(true);

                var othDir = new DirectoryInfo(projectPath + "\\.vs");
                othDir.Delete(true);

                File.Delete(projectPath + "\\WpfApp2.sln");
                File.Delete(projectPath + "\\WpfApp2.sln.DotSettings.user");

            }
            catch (Exception e)
            { }
        }
        #endregion

        //Building .exe
        #region
        public void StartBuild(string fromPath)
        {
            string toPath = GetDirPath();

            CreateFileToBuild();

            //get sln to build
            //DirectoryInfo dir = new DirectoryInfo(fromPath);
            //FileInfo[] files = dir.GetFiles("*.sln");
            //string projectFileName = files[0].FullName;

            //rebuild
            //ExecuteCommandSync(@"C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe "+projectFileName);

            //RemoveBuildFile(toPath);
        }
        
        public static void ExecuteCommandSync(object command)
        {
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run, and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows, and then exit.
                System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);
                // The following commands are needed to redirect the standard output. 
                //This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();

                // Get the output into a string
                string result = proc.StandardOutput.ReadToEnd();

                // Display the command output.
                Console.WriteLine(result);
            }
            catch (Exception objException)
            {
                // Log the exception
            }
        }
        #endregion
    }
}
