using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBTerminal.Core.Interfaces;

namespace USBTerminal.Core.Utils
{
    // provides absolute path for shared resources C:/...../Resources
    public class AbsoluteResourcePathHelper : IAbsoluteResourcePathHelper
    {
        public string GetAbsolutePath(string fileName)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            
            var projectRootUri = GetNThParent(currentDirectory, 5);
            var absolutePath = Path.Combine(projectRootUri.ToString(), "USBTerminal.Core", "Resources", fileName);
            return absolutePath;
        }

        private string GetNThParent(string path, int levels)
        {
            DirectoryInfo directoryInfo = Directory.GetParent(path);
            for (int i = 0; i < levels-1; i++)
            {
                directoryInfo = directoryInfo?.Parent;
            }
            return directoryInfo?.FullName;
        }
    }
}
