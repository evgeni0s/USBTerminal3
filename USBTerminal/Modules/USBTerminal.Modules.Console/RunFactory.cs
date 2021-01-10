using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;
using USBTerminal.Core.Enums.Console;
using USBTerminal.Core.Interfaces.Console;

namespace USBTerminal.Modules.Console
{
    public class RunFactory : IRunFactory
    {
        public IRun Get(RunType runType)
        {
            switch (runType)
            {
                case RunType.White:
                    return new WhiteRun();
                case RunType.Blue:
                    return new BlueRun();
                case RunType.Green:
                    return new GreenRun();
                case RunType.Yellow:
                    return new YellowRun();
                case RunType.Orange:
                    return new OrangeRun();
                case RunType.Red:
                    return new RedRun();
                default:
                    throw new NotImplementedException($"Run type {runType} is not supported.");
            }
        }
    }

    public class WhiteRun : Run, IRun { }
    public class BlueRun : Run, IRun { }
    public class GreenRun : Run, IRun { }
    public class YellowRun : Run, IRun { }
    public class OrangeRun : Run, IRun { }
    public class RedRun : Run, IRun { }
}
