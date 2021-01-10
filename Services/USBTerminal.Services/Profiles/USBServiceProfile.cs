using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using USBTerminal.Services.Interfaces.Models;

namespace USBTerminal.Services.Profiles
{
    public class USBServiceProfile: Profile
    {
        public USBServiceProfile()
        {
            CreateMap<string, Parity>().ConvertUsing(Convert);
            CreateMap<Parity, string>().ConvertUsing(Convert);

            CreateMap<double, StopBits>().ConvertUsing(Convert);
            CreateMap<StopBits, double>().ConvertUsing(Convert);
            CreateMap<string, StopBits>().ConvertUsing(Convert);
            CreateMap<StopBits, string>().ConvertUsing(Convert);

            CreateMap<SerialPort, SerialPortModel>();
            CreateMap<SerialPortModel, SerialPort>();
        }

        public Parity Convert(string source, Parity destination, ResolutionContext context)
        {
            return Enum.TryParse(source, out destination) ?
                destination
                : throw new ArgumentException($"Parity could not be parsed: {source}");
        }

        public string Convert(Parity source, string destination, ResolutionContext context)
        {
            switch (source)
            {
                case Parity.None:
                    return "None";
                case Parity.Odd:
                    return "Odd";
                case Parity.Even:
                    return "Even";
                case Parity.Mark:
                    return "Mark";
                case Parity.Space:
                    return "Space";
                default:
                    throw new ArgumentException($"Parity could not be parsed to string: {source}");
            }
        }

        public StopBits Convert(double source, StopBits destination, ResolutionContext context)
        {
            if (source == 0) return StopBits.None;
            if (source == 1) return StopBits.One;
            if (source == 2) return StopBits.Two;
            if (source == 1.5) return StopBits.OnePointFive;
            throw new ArgumentException($"StopBits could not be parsed: {source}");
        }

        public double Convert(StopBits source, double destination, ResolutionContext context)
        {
            switch (source)
            {
                case StopBits.None:
                    return 0;
                case StopBits.One:
                    return 1;
                case StopBits.Two:
                    return 2;
                case StopBits.OnePointFive:
                    return 1.5;

            }
            throw new ArgumentException($"StopBits could not be parsed: {source}");
        }

        // string -> double -> enum StopBits
        public string Convert(StopBits source, string destination, ResolutionContext context)
        {
            return Convert(source, 0, context).ToString();
        }

        // string -> double -> enum StopBits
        public StopBits Convert(string source, StopBits destination, ResolutionContext context)
        {
            if (double.TryParse(source, out double stopBitsDouble))
                return Convert(stopBitsDouble, destination, context);
            throw new ArgumentException($"StopBits could not be parsed: {source}");
        }
    }
    // Alternative
    //public class ParityConverter : ITypeConverter<string, Parity>, ITypeConverter<Parity, string>
    //{
    //    public Parity Convert(string source, Parity destination, ResolutionContext context)
    //    {
    //        return Enum.TryParse(source, out destination) ? 
    //            destination 
    //            : throw new ArgumentException($"Parity could not be parsed: {source}");
    //    }

    //    public string Convert(Parity source, string destination, ResolutionContext context)
    //    {
    //        switch (source)
    //        {
    //            case Parity.None:
    //                return "None";
    //            case Parity.Odd:
    //                return "Odd";
    //            case Parity.Even:
    //                return "Even";
    //            case Parity.Mark:
    //                return "Mark";
    //            case Parity.Space:
    //                return "Space";
    //            default:
    //                throw new ArgumentException($"Parity could not be parsed to string: {source}");
    //        }
    //    }
    //}

}
