using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBTerminal.Core.Mvvm;

namespace USBTerminal.Modules.USB.ViewModels
{
    public class USBPortViewModel : ViewModelBase
    {
        private string _name;
        private string _boudRate;
        private string _parity;
        private string _dataBits;
        private string _stopBits;
        private string _dataMode;

        public USBPortViewModel()
        {
        }

        public string Name
        {
            get => _name;
            set
            {
                SetProperty(ref _name, value);
            }
        }

        public string BoudRate
        {
            get => _boudRate;
            set
            {
                SetProperty(ref _boudRate, value);
            }
        }

        public string Parity
        {
            get => _parity;
            set
            {
                SetProperty(ref _parity, value);
            }
        }

        public string DataBits
        {
            get => _dataBits;
            set
            {
                SetProperty(ref _dataBits, value);
            }
        }

        public string StopBits
        {
            get => _stopBits;
            set
            {
                SetProperty(ref _stopBits, value);
            }
        }
        public string DataMode
        {
            get => _dataMode;
            set
            {
                SetProperty(ref _dataMode, value);
            }
        }

    }
}
