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
        private string FilePath;
        private List<MainWindow.fileData> BgContent = new List<MainWindow.fileData>();
        private List<MainWindow.fileData> SpriteContent = new List<MainWindow.fileData>();
        private List<MainWindow.fileData> MusicContent = new List<MainWindow.fileData>();

        public struct vnContent
        {
            public string ContentName { get; set; }
            public string ContentText { get; set; }       
        }

        public MiraiVn(string path,List<MainWindow.fileData> bg,List<MainWindow.fileData> sprite,List<MainWindow.fileData> muisc)
        {
            FilePath = path;
            BgContent = bg;
            SpriteContent = sprite;
            MusicContent = muisc;
        }

        public List<MainWindow.eventHierarchy> GetProjectLines(out int LineEnd)
        {
            List<MainWindow.eventHierarchy> projectLines = new List<MainWindow.eventHierarchy>();
            List<MainWindow.eventHierarchy> TempLinesArray = null;

            string[] tempStr =  File.ReadAllLines(FilePath);

            int lineCount = 0;
            int sceneCount = 0;

            string tempName=null;

            foreach (string line in tempStr)
            {
                if (line != "...")
                {                   
                    if (line.Contains("new-"))
                    {
                        tempName = line.Replace("new-", "");

                        //try to fix
                        TempLinesArray = new List<MainWindow.eventHierarchy>();
                    }
                    else if (line == "end")
                    {
                        projectLines.Add(new MainWindow.eventHierarchy {sceneName = tempName, EventContetn = TempLinesArray,sceneCount = sceneCount});
                        sceneCount++;
                    }
                    else
                    {
                        TempLinesArray.Add(new MainWindow.eventHierarchy{sceneName = line,sceneCount = lineCount});
                    }                    
                    lineCount++;
                }
                else
                {
                    break;
                }
            }
            LineEnd = lineCount-1;
            return projectLines;
        }

        public void SetResources(ref string[] ProjectLines,ref int LineReaded,ref string []sprite,ref string bg, ref string music,ref List<vnContent> dialogue)
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

                //manage music
                else if(TempCommand.Contains("#Add music"))
                {
                    var result = MusicContent.Find(x => x.path.Contains(TempCommand.Replace("#Add music=", "")));
                    music = result.path;
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
