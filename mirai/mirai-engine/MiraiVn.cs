using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace mirai_engine
{
    public class MiraiVn
    {
        string FilePath;
        List<MainWindow.fileContent> BgContent = new List<MainWindow.fileContent>();
        List<MainWindow.fileContent> SpriteContent = new List<MainWindow.fileContent>();

        public struct vnContent
        {
            public string ContentName { get; set; }
            public string ContentText { get; set; }       
        }

        public MiraiVn(string path,List<MainWindow.fileContent> bg,List<MainWindow.fileContent> sprite)
        {
            FilePath = path;
            BgContent = bg;
            SpriteContent = sprite;
        }

        public void SetResources(ref string[] ProjectLines,ref int LineReaded,ref string []sprite,ref string bg,ref List<vnContent> dialogue)
        {      
            int spriteCount = 0;
            string TempName=null;
            string TempCommand= "#Add";

            while(TempCommand.Contains("#Add")&&TempCommand!=null)
            {
                TempCommand = ProjectLines[LineReaded];

                //manage images
                if (TempCommand.Contains("#Add bg"))
                {
                    var result = BgContent.Find(x => x.path.Contains(TempCommand.Replace("#Add bg=", "")));
                    bg = result.path;
                }
                else if(TempCommand.Contains("#Add sprite"))
                {
                    var result = SpriteContent.Find(x => x.path.Contains(TempCommand.Replace("#Add sprite=", "")));
                    sprite[spriteCount]= result.path;

                    spriteCount++;
                }

                //manage command(event)
                else if(TempCommand.Contains("#Add event=Hide"))
                {                   
                    dialogue.Add(new vnContent { ContentName = "#Hide", ContentText = TempCommand.Replace("#Add event=Hide.","")});
                }
                else if(TempCommand.Contains("#Add event=Show1"))
                {
                    dialogue.Add(new vnContent { ContentName = "#Show1", ContentText = TempCommand.Replace("#Add event=Show1.", "") });
                }
                else if (TempCommand.Contains("#Add event=Show2"))
                {
                    dialogue.Add(new vnContent { ContentName = "#Show2", ContentText = TempCommand.Replace("#Add event=Show2.", "") });
                }

                //manage text
                else if(TempCommand.Contains("#Add name"))
                {
                    if(TempCommand.Replace("#Add name=","")!=TempName)
                    {
                        TempName = TempCommand.Replace("#Add name=", "");
                    }
                }
                else if(TempCommand.Contains("#Add text"))
                {
                    dialogue.Add(new vnContent { ContentName = TempName, ContentText = TempCommand.Replace("#Add text=", "")});                    
                }

                else if (TempCommand.Equals("end"))
                {
                    break;
                }

                LineReaded++;
            }            
        }


    }
}
