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

            //temp values
            //ImgPath = VnContent[0].Path;
            //TextPath = VnContent[1].Path;

            LoadNovel();

            //LoadText();

            StartNovel();
        }      

        void LoadNovel()
        {
            string[] lines = File.ReadAllLines(VnPath);
            int count = 0;

            ProjectLines = new string[lines.Length];
            
            //filter
            foreach (string line in lines)
            {
                if(line!="")
                {
                    ProjectLines[count] = line;
                    count++;
                }
            }

            string tempCommand="val";
            
            while(tempCommand!=null||tempCommand!="...")
            {
                tempCommand = ProjectLines[LineReaded];
                Console.WriteLine();               
                if (tempCommand.Contains("new-scene"))
                {
                    LineReaded++;
                    mirai.SetResources(ref ProjectLines, ref LineReaded, ref VnSprite, ref VnBg, ref DialogueVn);
                }      
                else
                {
                    break;
                }
            }
        }

        void StartNovel()
        {
            BitmapImage bgImage = new BitmapImage();
            bgImage.BeginInit();        
            bgImage.UriSource = new Uri(VnBg, UriKind.Absolute);
            bgImage.EndInit();            
            backgroundImg.Source = bgImage;

            BitmapImage sprite1Image = new BitmapImage();
            sprite1Image.BeginInit();
            sprite1Image.UriSource = new Uri(VnSprite[0], UriKind.Absolute);
            sprite1Image.EndInit();
            spriteImg1.Source = sprite1Image;

            BitmapImage sprite2Image = new BitmapImage();
            sprite2Image.BeginInit();
            sprite2Image.UriSource = new Uri(VnSprite[1], UriKind.Absolute);
            sprite2Image.EndInit();
            spriteImg2.Source = sprite2Image;
        }

        protected int count = 0;

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (count < DialogueVn.Count)
            {
                NameTextBox.Clear();
                NameTextBox.AppendText(DialogueVn[count].CharName);

                if (count > 0 && DialogueVn[count].CharName != DialogueVn[count - 1].CharName)
                {
                    TextBox.Clear();
                }

                //link lines

                else
                {
                    TextBox.AppendText(" ");
                }

                TextBox.AppendText(DialogueVn[count].Dialogue);

                count++;
            }
        }

        //void LoadText()
        //{
        //    //set default read position
        //    count = 0;

        //    StreamReader reader = new StreamReader(VnPath);

        //    string str = reader.ReadToEnd();

        //    //remove empty lines
        //    str = Regex.Replace(str, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);

        //    string[] Dialoge = Regex.Split(str, "\r\n");

        //    string tempNameStr = null;

        //    foreach (var line in Dialoge)
        //    {
        //        if (line.StartsWith("~"))
        //        {
        //            tempNameStr = line.Remove(0, 1);
        //        }
        //        else
        //        {
        //            parsedtext.Add(new ParsedText() { CharName = tempNameStr, Dialoge = line });
        //        }
        //    }
    
    }
}
