using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace USBTerminal.Core.Interfaces.Console
{
    // Interface is in Core project as exception, because it wired up to Logger
    public interface ITextBoxSink: ILogEventSink
    {
    }
}
