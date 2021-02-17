using System;

namespace USBTerminal.Core.Interfaces
{
    public interface IAbsoluteResourcePathHelper
    {
        Uri GetAbsolutePath(string fileName);
    }
}