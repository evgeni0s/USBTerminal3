using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBTerminal.Services.Interfaces.Models.SesameBot.Properties;
using USBTerminal.Services.SeasameService.GridViewCells;

namespace USBTerminal.Services.SeasameService
{
    public class ComponentProperties: IComponentProperties
    {
        public Guid Id { get; set; }
        public List<IGridViewField> Properties { get; set; }
    }
}
