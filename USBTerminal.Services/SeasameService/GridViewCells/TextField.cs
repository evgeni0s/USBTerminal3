using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBTerminal.Services.Interfaces.Models.SesameBot.Properties;

namespace USBTerminal.Services.SeasameService.GridViewCells
{
    public class TextField : GridViewField, ITextField
    {
        public override int CompareTo(IGridViewField other)
        {
            return base.CompareTo(other);
        }
    }
}
