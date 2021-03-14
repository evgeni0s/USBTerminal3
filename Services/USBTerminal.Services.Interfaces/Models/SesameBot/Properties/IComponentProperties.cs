﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBTerminal.Services.Interfaces.Models.SesameBot.Properties
{
    public interface IComponentProperties
    {
        Guid Id { get; set; }
        List<IGridViewField> Properties { get; }
    }
}
