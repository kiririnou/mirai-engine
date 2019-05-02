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
        MiraiVn vn;
        List<MainWindow.fileContent> VnContent = new List<MainWindow.fileContent>();
      
        public struct ParsedText
        {
            public string CharName { get; set; }
            public string Dialoge { get; set; }
        }

        protected List<ParsedText> parsedtext = new List<ParsedText>();

        //temp values
        string ImgPath;
        string TextPath;        

        public DebugWindow(MiraiVn novel, List<MainWindow.fileContent> content)
        {
            InitializeComponent();

            vn = novel;
            VnContent = content;

            //temp values
            ImgPath = VnContent[0].Path;
            TextPath = VnContent[1].Path;

            LoadText();
          
            StartNovel();
        }

        void StartNovel()
        {
            BitmapImage bmImage = new BitmapImage();
            bmImage.BeginInit();        
            bmImage.UriSource = new Uri(ImgPath, UriKind.Absolute);
            bmImage.EndInit();            
            background.Source = bmImage;                  
        }

        protected int count;

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            NameTextBox.Clear();
            NameTextBox.AppendText(parsedtext[count].CharName);

            if (count > 0 && parsedtext[count].CharName != parsedtext[count - 1].CharName)
            {
                TextBox.Clear();
            }

            //link lines

            else if (parsedtext[count].Dialoge.Contains("/"))
            {          
                TextBox.AppendText(" ");
            }

            TextBox.AppendText(parsedtext[count].Dialoge.Replace("/", ""));

            count++;
        }

        void LoadText()
        {
            //set default read position
            count = 0;

            StreamReader reader = new StreamReader(TextPath);

            string str = reader.ReadToEnd();

            //remove empty lines
            str = Regex.Replace(str, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);

            string[] Dialoge = Regex.Split(str, "\r\n");

            string tempNameStr = null;

            foreach (var line in Dialoge)
            {
                if (line.StartsWith("~"))
                {
                    tempNameStr = line.Remove(0, 1);
                }
                else
                {
                    parsedtext.Add(new ParsedText() { CharName = tempNameStr, Dialoge = line });
                }
            }
        }      
    }
}
