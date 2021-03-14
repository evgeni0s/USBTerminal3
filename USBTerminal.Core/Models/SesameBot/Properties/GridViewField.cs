using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBTerminal.Services.SeasameService.GridViewCells
{
    public class GridViewField : IComparable<GridViewField>
    {
        // Smaller goes last
        private static List<string> sortingOrder = new List<string> { "Type", "Name"};
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsReadOnly { get; set; }

        public virtual int CompareTo(GridViewField other)
        {
            var priorityThis = sortingOrder.IndexOf(Name);
            var priorityOther = sortingOrder.IndexOf(other.Name);
            //if (priorityOther == -1)
            //{
            //    return -1;
            //}
            return priorityThis < priorityOther ? 1 : -1;
        }
    }
}
