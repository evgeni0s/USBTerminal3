using System;

namespace USBTerminal.Core.Interfaces
{
    public interface IAbsoluteResourcePathHelper
    {
        string GetAbsolutePath(string fileName);
    }
}