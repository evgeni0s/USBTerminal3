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
        public Uri GetAbsolutePath(string fileName)
        {
            var videoPath = Directory.GetCurrentDirectory();
            var projectRootUri = GetNThParent(new Uri(videoPath), 5);
            var absolutePath = Path.Combine(projectRootUri.ToString(), "USBTerminal.Core", "Resources", fileName);
            return new Uri(absolutePath);
        }

        private Uri GetNThParent(Uri uri, int levels)
        {

            Uri result = uri;
            for (int i = 0; i < levels; i++)
            {
                result = GetParentUriString(result);
            }
            return result;
        }

        private Uri GetParentUriString(Uri uri)
        {
            return new Uri(uri.AbsoluteUri.Remove(uri.AbsoluteUri.Length - uri.Segments.Last().Length));
        }
    }
}
