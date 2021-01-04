using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace USBTerminal.Core.Interfaces.Console
{
    public interface ITextBoxLogger: ILogger
    {
        public string GetText();
    }
}
