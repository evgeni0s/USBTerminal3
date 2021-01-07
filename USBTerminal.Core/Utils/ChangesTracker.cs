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
        }

        public IEnumerable<T2> AddedItems
        {
            get => newValues.Where(n => oldValues.All(o => !areEqual(o, n)));
        }

        public IEnumerable<T1> RemovedItems
        {
            get => oldValues.Where(n => newValues.All(o => !areEqual(n, o)));
        }

        public IEnumerable<T1> UpdatedItems
        {
            get => oldValues.Where(n => newValues.Any(o => areEqual(n, o)));
        }
    }
}
