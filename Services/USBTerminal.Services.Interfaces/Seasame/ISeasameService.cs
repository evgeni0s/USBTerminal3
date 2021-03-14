using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBTerminal.Services.Interfaces.Seasame
{
    public interface ISeasameService
    {
        //public void MoveLeft();
        //public void MoveRight();
        public void MoveTo(double percent);
        public void GetWiFiInfo();
    }
}
