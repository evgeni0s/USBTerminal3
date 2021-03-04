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

        #region USB
        CompositeCommand OpenPortCommand { get; }
        CompositeCommand ClosePortCommand { get; }
        CompositeCommand SendMessageToPortCommand { get; }
        #endregion

        #region Network
        CompositeCommand ScanNetworkCommand { get; }
        CompositeCommand OpenNetworkConnectionCommand { get; }
        CompositeCommand CloseNetworkConnectionCommand { get; }
        CompositeCommand SendMessageOnNetworkCommand { get; }

        #endregion

        #region Seasame
        CompositeCommand SearchSeasameBotsOnNetworkCommand { get; }
        #endregion
    }
}
