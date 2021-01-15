using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace USBTerminal.Core.Interfaces
{
    public interface IApplicationCommands
    {
        CompositeCommand TerminalCommand { get; }
        CompositeCommand LoggingCommand { get; }
        CompositeCommand OpenPortCommand { get; }
        CompositeCommand ClosePortCommand { get; }
        CompositeCommand SendMessageToPortCommand { get; }
    }
}
