using System;
using System.Collections.Generic;
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
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using WMPLib;
using System.Reflection;
using Path = System.IO.Path;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for DebugWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public struct fileData
        {
            public string dir { get; set; }
            public string path { get; set; }
        }
        public struct vnContent
        {
            public string ContentName { get; set; }
            public string ContentText { get; set; }
        }

        List<fileData> BgContent = new List<fileData>();
        List<fileData> SpriteContent = new List<fileData>();
        List<fileData> MusicContent = new List<fileData>();
        string VnPath;
        string[] ProjectLines;

        WindowsMediaPlayer wplayer = new WindowsMediaPlayer();

        //temp window resouces
        List<vnContent> DialogueVn = new List<vnContent>();
        string[] VnSprite = new string[10];
        string VnBg;
        string VnMusic;
        int LineReaded;

        public MainWindow()
        {
            InitializeComponent();
            GetFiles();
        }      

        public void GetFiles()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "")+"\\res";
            
            LineReaded = 0;
            VnPath = path;

            foreach (var drive in Directory.GetDirectories(path))
            {                          
                DirectoryInfo dir = new DirectoryInfo(drive);
                FileInfo[] Files = dir.GetFiles();

                if (Path.GetFileName(drive) == "Bg")
                {
                    foreach (var file in Files)
                    {
                        BgContent.Add(new fileData { dir = Path.GetFileName(drive), path = file.FullName });
                    }
                }
                else if (Path.GetFileName(drive) == "Sprite")
                {
                    foreach (var file in Files)
                    {
                        SpriteContent.Add(new fileData { dir = Path.GetFileName(drive), path = file.FullName });
                    }
                }
                else if (Path.GetFileName(drive) == "Music")
                {
                    foreach (var file in Files)
                    {
                        MusicContent.Add(new fileData { dir = Path.GetFileName(drive), path = file.FullName });
                    }
                }
            }
             LoadNovel();
        }
 

        void LoadNovel()
        {
            if (LineReaded == 0)
            {
                string[] lines = File.ReadAllLines(VnPath+"\\script.txt");
                int count = 0;
                ProjectLines = new string[lines.Length];

                //filter
                foreach (string line in lines)
                {
                    if (line != "")
                    {
                        ProjectLines[count] = line;
                        count++;
                    }
                }
            }

            string tempCommand="val";
            
            while(tempCommand!=null)
            {
                tempCommand = ProjectLines[LineReaded];
             
                if (tempCommand.Contains("new-scene"))
                {                   
                    LineReaded++;
                    setResources(ref ProjectLines, ref LineReaded, ref VnSprite, ref VnBg, ref VnMusic, ref DialogueVn);

                    StartNovel();
                }                                  
                else
                {
                    break;
                }
            }
        }

        public void setResources(ref string[] ProjectLines, ref int LineReaded, ref string[] sprite, ref string bg, ref string music, ref List<vnContent> dialogue)
        {
            int spriteCount = 0;
            string TempName = null;
            string TempCommand = "#Add";

            while (TempCommand.Contains("#Add") && TempCommand != null)
            {
                TempCommand = ProjectLines[LineReaded];

                //manage images
                if (TempCommand.Contains("#Add bg"))
                {
                    var result = BgContent.Find(x => x.path.Contains(TempCommand.Replace("#Add bg=", "")));
                    bg = result.path;
                }
                else if (TempCommand.Contains("#Add sprite"))
                {
                    var result = SpriteContent.Find(x => x.path.Contains(TempCommand.Replace("#Add sprite=", "")));
                    sprite[spriteCount] = result.path;

                    spriteCount++;
                }

                //manage muisc
                else if (TempCommand.Contains("#Add muisc"))
                {
                    var result = MusicContent.Find(x => x.path.Contains(TempCommand.Replace("#Add muisc=", "")));
                    music = result.path;
                }

                //manage command(event)
                else if (TempCommand.Contains("#Add event=Hide"))
                {
                    dialogue.Add(new vnContent { ContentName = "#Hide", ContentText = TempCommand.Replace("#Add event=Hide.", "") });
                }
                else if (TempCommand.Contains("#Add event=Show1"))
                {
                    dialogue.Add(new vnContent { ContentName = "#Show1", ContentText = TempCommand.Replace("#Add event=Show1.", "") });
                }
                else if (TempCommand.Contains("#Add event=Show2"))
                {
                    dialogue.Add(new vnContent { ContentName = "#Show2", ContentText = TempCommand.Replace("#Add event=Show2.", "") });
                }

                //manage text
                else if (TempCommand.Contains("#Add name"))
                {
                    if (TempCommand.Replace("#Add name=", "") != TempName)
                    {
                        TempName = TempCommand.Replace("#Add name=", "");
                    }
                }
                else if (TempCommand.Contains("#Add text"))
                {
                    dialogue.Add(new vnContent { ContentName = TempName, ContentText = TempCommand.Replace("#Add text=", "") });
                }

                else if (TempCommand.Equals("end"))
                {
                    break;
                }

                LineReaded++;
            }
        }

        void SetResouces(Image img,string imgPath)
        {
            BitmapImage bgImage = new BitmapImage();
            bgImage.BeginInit();
            bgImage.UriSource = new Uri(imgPath, UriKind.Absolute);
            bgImage.EndInit();
            img.Source = bgImage;
        }

        void StartNovel()
        {
            dialogueCount = 0;
            //bg
            SetResouces(backgroundImg, VnBg);

            //muisc
            if (VnMusic != null)
            {
                wplayer.URL = VnMusic;
                wplayer.controls.play();
            }
            else
            {
                wplayer.controls.stop();
            }

            //sprite
            if (VnSprite[0] != null)
            {
                SetResouces(SpriteImg1, VnSprite[0]);
            }
            else
            {
                SpriteImg1.Source = null;
            }
            if (VnSprite[1] != null)
            {
                SetResouces(SpriteImg2, VnSprite[1]);
            }
            else
            {
                SpriteImg2.Source = null;
            }
        }

        protected int dialogueCount;

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (dialogueCount < DialogueVn.Count)
            {
                //handle text
                if (!DialogueVn[dialogueCount].ContentName.Contains("#"))
                {
                    NovelText(DialogueVn[dialogueCount].ContentName, DialogueVn[dialogueCount].ContentText);
                    dialogueCount++;
                }
                //handle events
                else if (DialogueVn[dialogueCount].ContentName.Contains("#"))
                {
                    NovelEvent(DialogueVn[dialogueCount].ContentName, DialogueVn[dialogueCount].ContentText);               
                    dialogueCount++;
                }
            }           
            else
            {
                //load new scene
                if (ProjectLines[LineReaded+1] != null)
                {
                    NameTextBox.Clear();
                    TextBox.Clear();

                    DialogueVn.Clear();
                    VnSprite[0] = null;
                    VnSprite[1] = null;
                    VnBg = null;
                    VnMusic = null;

                    LineReaded++;
                    LoadNovel();
                }
            }
        }
        
        void NovelText(string name, string text)
        {
            NameTextBox.Clear();
            NameTextBox.AppendText(name);

            if (dialogueCount > 0 && name != DialogueVn[dialogueCount - 1].ContentName)
            {
                TextBox.Clear();
            }
            else
            {
                TextBox.AppendText(" ");
            }

            TextBox.AppendText(DialogueVn[dialogueCount].ContentText);        
        }

        void NovelEvent(string commandName,string commandTemp)
        {
            switch (commandName)
            {
                case ("#Hide"):
                    {
                        if (commandTemp.Equals("1"))
                        {
                            SpriteImg1.Source = null;
                        }
                        else if (commandTemp.Equals("2"))
                        {
                            SpriteImg2.Source = null;
                        }
                        break;
                    }
                case ("#Show1"):
                    {
                        int pos = Int32.Parse(commandTemp);

                        BitmapImage sprite1Image = new BitmapImage();
                        sprite1Image.BeginInit();
                        sprite1Image.UriSource = new Uri(VnSprite[pos-1], UriKind.Absolute);
                        sprite1Image.EndInit();
                        SpriteImg1.Source = sprite1Image;
                        break;
                    }
                case ("#Show2"):
                    {
                        int pos = Int32.Parse(commandTemp);

                        BitmapImage sprite2Image = new BitmapImage();
                        sprite2Image.BeginInit();
                        sprite2Image.UriSource = new Uri(VnSprite[pos-1], UriKind.Absolute);
                        sprite2Image.EndInit();
                        SpriteImg2.Source = sprite2Image;
                        break;
                    }
            }
        }
       
    }
}
