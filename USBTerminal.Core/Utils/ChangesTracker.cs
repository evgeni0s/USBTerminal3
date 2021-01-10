using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USBTerminal.Core.Utils
{
    public class ChangesTracker<T1, T2>
    {
        private readonly IEnumerable<T1> oldValues;
        private readonly IEnumerable<T2> newValues;
        private readonly Func<T1, T2, bool> areEqual;

        public ChangesTracker(IEnumerable<T1> oldValues, IEnumerable<T2> newValues, Func<T1, T2, bool> areEqual)
        {
            this.oldValues = oldValues;
            this.newValues = newValues;
            this.areEqual = areEqual;
            Refresh();
        }

        public void Refresh()
        {
            AddedItems = newValues.Where(n => oldValues.All(o => !areEqual(o, n))).ToList();
            RemovedItems = oldValues.Where(n => newValues.All(o => !areEqual(n, o))).ToList();
            UpdatedItems = oldValues.Where(n => newValues.Any(o => areEqual(n, o))).ToList();
        }

        public IEnumerable<T2> AddedItems { get; set; }

        public IEnumerable<T1> RemovedItems { get; set; }

        public IEnumerable<T1> UpdatedItems { get; set; }

        public T1 Match(T2 newValue)
        {
            return oldValues.FirstOrDefault(o => areEqual(o, newValue));
        }

        public T2 Match(T1 oldValue)
        {
            return newValues.FirstOrDefault(n => areEqual(oldValue, n));
        }
    }
}
