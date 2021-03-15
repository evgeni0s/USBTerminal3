using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Input;

namespace InWit.ViewModel.Utils
{
    /// <summary>
    /// Class extends <see cref="System.Windows.Interactivity.TriggerAction"/> behavior to pass parameters to commands associate with GUI event
    /// </summary>
    public class InvokeCommandActionWithParam : TriggerAction<DependencyObject>
    {
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(InvokeCommandActionWithParam), null);

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(InvokeCommandActionWithParam), null);

        public static readonly DependencyProperty InvokeParameterProperty = DependencyProperty.Register(
            "InvokeParameter", typeof(object), typeof(InvokeCommandActionWithParam), null);

        /// <summary>
        /// Invoke parametr property
        /// </summary>
        public object InvokeParameter
        {
            get { return GetValue(InvokeParameterProperty); }
            set { SetValue(InvokeParameterProperty, value); }
        }
        /// <summary>
        /// Command to invoke
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        /// <summary>
        /// Command parametr
        /// </summary>
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        protected override void Invoke(object parameter)
        {
            //Allows binding to invoke paramater and converting it to command parameter
            InvokeParameter = parameter;

            if (Command.CanExecute(CommandParameter))
                Command.Execute(CommandParameter);
        }
    }
}

