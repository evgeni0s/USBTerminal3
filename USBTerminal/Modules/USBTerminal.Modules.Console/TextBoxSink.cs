using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;
using USBTerminal.Core.Interfaces;
using USBTerminal.Core.Interfaces.Console;

namespace USBTerminal.Modules.Console
{
    public class TextBoxSink : ITextBoxSink
    {
        private readonly IApplicationCommands applicationCommands;

        public TextBoxSink(IApplicationCommands applicationCommands)
        {
            this.applicationCommands = applicationCommands;
        }

        public void Emit(LogEvent logEvent)
        {
            applicationCommands.LoggingCommand.Execute(logEvent);
        }
    }
}
