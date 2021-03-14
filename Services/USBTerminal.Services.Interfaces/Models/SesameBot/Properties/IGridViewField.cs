using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBTerminal.Services.Interfaces.Models.SesameBot.Properties
{
    public interface IGridViewField : IComparable<IGridViewField>
    {
        string Name { get; set; }
    }
}
