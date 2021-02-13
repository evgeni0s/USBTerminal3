using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBTerminal.Modules.Console
{
    public static class NavigationArguments
    {
        public static NavigationParameters TerminalParameters = new NavigationParameters { { "Title", "Terminal" } };
        public static NavigationParameters LogsParameters = new NavigationParameters { { "Title", "Application Logs" } };

    }
}
