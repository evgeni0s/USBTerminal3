using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FizzWare.NBuilder;

namespace USBTerminal.Core.Tests
{
    public static class ChangesTrackerMockups
    {
        public static IEnumerable<A> GetAList(int length)
        {
            return Enumerable.Range(1, length).Select(GetA);
        }

        public static IEnumerable<B> GetBList(int length)
        {
            return Enumerable.Range(1, length).Select(GetB);
        }

        public static A GetA(int id)
        {
            var a = Builder<A>.CreateNew().Build();
            a.Id = id;
            return a;
        }


        public static B GetB(int id)
        {
            var b = Builder<B>.CreateNew().Build();
            b.Iddd = id;
            return b;
        }
    }

    public class A
    {
        public int Id { get; set; }
        public string Length { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public int Density { get; set; }
    }


    public class B
    {
        public int Iddd { get; set; }
        public string Color { get; set; }
        public int Taste { get; set; }
        public int Smell { get; set; }
        public int Price { get; set; }
    }
}
