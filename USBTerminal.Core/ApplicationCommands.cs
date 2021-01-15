using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using USBTerminal.Core.Interfaces;

namespace USBTerminal.Core
{
    public class ApplicationCommands: IApplicationCommands
    {
        public CompositeCommand TerminalCommand { get; } = new CompositeCommand();
        public CompositeCommand LoggingCommand { get; } = new CompositeCommand();
        public CompositeCommand OpenPortCommand { get; } = new CompositeCommand();
        public CompositeCommand ClosePortCommand { get; } = new CompositeCommand();
        public CompositeCommand SendMessageToPortCommand { get; } = new CompositeCommand();
    }
}
