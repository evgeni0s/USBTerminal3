using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Runtime.CompilerServices;

namespace USBTerminal.Core.Mvvm
{
    public abstract class ViewModelBase : BindableBase, IDestructible
    {
        protected ViewModelBase()
        {

        }

        public virtual void Destroy()
        {

        }
    }
}
