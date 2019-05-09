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

namespace mirai_engine
{
    /// <summary>
    /// Interaction logic for DebugWindow.xaml
    /// </summary>
    public partial class DebugWindow : Window
    {
        MiraiVn mirai;
        List<MainWindow.fileContent> BgContent = new List<MainWindow.fileContent>();
        List<MainWindow.fileContent> SpriteContent = new List<MainWindow.fileContent>();
        string VnPath;
        string[] ProjectLines;      

        //temp window resouces
        List<MiraiVn.vnContent> DialogueVn = new List<MiraiVn.vnContent>();
        string[] VnSprite = new string[10];
        string VnBg;

        int LineReaded;

        public DebugWindow(MiraiVn novel, string path, List<MainWindow.fileContent> bg,List<MainWindow.fileContent> sprite)
        {
            InitializeComponent();

            mirai = novel;
            VnPath = path;
            BgContent = bg;
            SpriteContent = sprite;
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
             
                if (tempCommand.Equals("new-scene"))
                {                   
                    LineReaded++;
                    mirai.SetResources(ref ProjectLines, ref LineReaded, ref VnSprite, ref VnBg, ref DialogueVn);

                    StartNovel();
                }                                  
                else
                {
                    break;
                }
            }
        }

        void StartNovel()
        {
            dialogueCount = 0;

            BitmapImage bgImage = new BitmapImage();
            bgImage.BeginInit();        
            bgImage.UriSource = new Uri(VnBg, UriKind.Absolute);
            bgImage.EndInit();            
            backgroundImg.Source = bgImage;

            if (VnSprite[0] != null)
            {
                BitmapImage sprite1Image = new BitmapImage();
                sprite1Image.BeginInit();
                sprite1Image.UriSource = new Uri(VnSprite[0], UriKind.Absolute);
                sprite1Image.EndInit();
                SpriteImg1.Source = sprite1Image;
            }
            else
            {
                SpriteImg1.Source = null;
            }

            if (VnSprite[1] != null)
            {
                BitmapImage sprite2Image = new BitmapImage();
                sprite2Image.BeginInit();
                sprite2Image.UriSource = new Uri(VnSprite[1], UriKind.Absolute);
                sprite2Image.EndInit();
                SpriteImg2.Source = sprite2Image;
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
