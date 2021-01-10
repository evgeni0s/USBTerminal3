using System;
using System.Collections.Generic;
using System.Text;
using USBTerminal.Core.Enums.Console;

namespace USBTerminal.Core.Interfaces.Console
{
    public interface IRunFactory
    {
        IRun Get(RunType runType);
    }
}
