using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mirai_engine
{
    public class MiraiVn
    {
        string FilePath;
        List<MainWindow.fileContent> VnContent = new List<MainWindow.fileContent>();

        public MiraiVn(string path,List<MainWindow.fileContent> content)
        {
            FilePath = path;
            VnContent = content;
        }     
    }
}
