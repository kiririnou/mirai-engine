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

namespace mirai_engine
{
    /// <summary>
    /// Interaction logic for DebugWindow.xaml
    /// </summary>
    public partial class DebugWindow : Window
    {
        readonly MiraiVn mirai;
        private List<MainWindow.fileData> BgContent = new List<MainWindow.fileData>();
        private List<MainWindow.fileData> SpriteContent = new List<MainWindow.fileData>();
        private List<MainWindow.fileData> MusicContent = new List<MainWindow.fileData>();
        readonly string VnPath;
        private string[] ProjectLines;

        private readonly WindowsMediaPlayer wplayer = new WindowsMediaPlayer();

        //temp window resources
        List<MiraiVn.vnContent> DialogueVn = new List<MiraiVn.vnContent>();
        string[] VnSprite = new string[10];
        private string VnBg;
        private string VnMusic;
        private int LineReaded;

        public DebugWindow(MiraiVn novel, string path, List<MainWindow.fileData> bg,List<MainWindow.fileData> sprite,List<MainWindow.fileData> music)
        {
            InitializeComponent();

            mirai = novel;
            VnPath = path;
            BgContent = bg;
            SpriteContent = sprite;
            MusicContent = music;
            LineReaded = 0;

            LoadNovel();          
        }      

        void LoadNovel()
        {
            if (LineReaded == 0)
            {
                string[] lines = File.ReadAllLines(VnPath);
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
                    mirai.SetResources(ref ProjectLines, ref LineReaded, ref VnSprite, ref VnBg, ref VnMusic, ref DialogueVn);

                    StartNovel();
                }                                  
                else
                {
                    break;
                }
            }
        }

        private void SetResouces(Image img,string imgPath)
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
