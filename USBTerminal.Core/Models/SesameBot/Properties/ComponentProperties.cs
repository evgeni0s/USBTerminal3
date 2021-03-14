using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBTerminal.Services.SeasameService.GridViewCells;

namespace USBTerminal.Services.SeasameService
{
    public class ComponentProperties
    {
        public Guid Id { get; set; }
        public List<GridViewField> Properties { get; set; }
    }
}
