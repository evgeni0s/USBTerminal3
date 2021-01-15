using Prism.Commands;
using Prism.Events;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using USBTerminal.Core.Enums.Console;
using USBTerminal.Core.Interfaces.Console;
using USBTerminal.Services.Interfaces.Events;

namespace USBTerminal.Modules.Console.Views
{
    /// <summary>
    /// Interaction logic for CustomRichTextBox.xaml
    /// </summary>
    public partial class CustomRichTextBox : RichTextBox
    {
        private List<Key> keysRequireFix = new List<Key>() { Key.Space, Key.Enter, Key.Back, Key.Delete, Key.Up, Key.Down };
        private Run _focusedInline;
        private bool isEnabledCustom = true;
        private readonly IRunFactory runFactory;
        private readonly IEventAggregator eventAggregator;

        public CustomRichTextBox(IRunFactory runFactory, IEventAggregator eventAggregator)
        {
            InitializeComponent();
            this.runFactory = runFactory;
            this.eventAggregator = eventAggregator;
        }


        #region processing key inputs
        private void subscribeEditableElement()
        {
            // this is where we handle the space and other keys wpf f*s up.
            System.Windows.Input.InputManager.Current.PreNotifyInput +=
                new NotifyInputEventHandler(PreNotifyInput);
            // This is where we handle all the rest of the keys
            TextCompositionManager.AddPreviewTextInputStartHandler(
                this,
                PreviewTextInputHandler);                                          ///OnPreviewKeyDown

            //SelectionChanged += (sender, e) => MoveCustomCaret();
            //LostFocus += (sender, e) => Caret.Visibility = Visibility.Collapsed;
            //GotFocus += (sender, e) => Caret.Visibility = Visibility.Visible;
        }

        //private void unSubscribeEditableElement() unsubscribe already tried. Probably need to solve other way

        private void PreNotifyInput(object sender, NotifyInputEventArgs e)
        {
            // I'm only interested in key down events
            if (e.StagingItem.Input.RoutedEvent != Keyboard.KeyDownEvent)
                return;
            var args = e.StagingItem.Input as KeyEventArgs;
            // I only care about the space key being pressed
            // you might have to check for other characters
            if (args == null || !keysRequireFix.Any(k => k == args.Key))
                return;

            if (!IsFocused)
                return;
            // stop event processing here
            args.Handled = true;
            // this is my internal method for handling a keystroke
            if (args.Key == Key.Space)
                HanleKeystroke(" ");
            else
                HandleKeyAction(args.Key);
        }

        private void PreviewTextInputHandler(object sender, TextCompositionEventArgs e)
        {
            HanleKeystroke(e.Text);
        }

        //Sends text from KEYBOARD to inputField. DO NOT USE FOR OTHER PURPOSES!!!
        private void HanleKeystroke(string p)
        {
            if (getPositionType(inputField) == CuretPositionType.Outside)
                CaretPosition = inputField.ContentEnd;


            // CaretPosition.InsertTextInRun(p);//Inserting text. Works but has issues with <- and -> . Acts Whiered
            //  CaretPosition.GetInsertionPosition(LogicalDirection.Backward).InsertTextInRun(p);
            // ScrollToEnd();

            //THIS CODE IS STABLE!!!!
            //http://www.java2s.com/Tutorial/CSharp/0470__Windows-Presentation-Foundation/ProgrammaticallyInsertTextintoaRichTextBox.htm
            TextPointer tp = CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            CaretPosition.InsertTextInRun(p);
            CaretPosition = tp;
            //STABLE REGION ENDS!

            ScrollToEnd();
        }


       // Sends actions to inputField
        private void HandleKeyAction(Key action)
        {
            switch (action)
            {
                case Key.Enter:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        CaretPosition.InsertTextInRun(Environment.NewLine);
                        ScrollToEnd();
                    }
                    else
                    {
                        Run run = CustomRun.Cmd;
                        run.Text = inputField.Text;
                        readOnlyItems.Inlines.Add(run);
                        readOnlyItems.Inlines.Add(new Run(Environment.NewLine));//problems with extra lines
                        eventAggregator.GetEvent<TerminalInputEvent>().Publish(inputField.Text);//runs command 
                        inputField.Text = "";
                        ScrollToEnd();
                        _focusedInline = null;
                    }
                    break;
                case Key.Delete:
                case Key.Back:
                    //Deletes text if selected
                    var deletableTextRange = GetSelection(inputField);
                    if (deletableTextRange != null)
                    {
                        deletableTextRange.Text = string.Empty;
                    }
                    else if (getPositionType(inputField) != CuretPositionType.Outside)
                    {
                        int deleteDirection = action == Key.Delete ? 1 : -1;
                        TextPointer tp = CaretPosition.GetPositionAtOffset(deleteDirection, LogicalDirection.Backward);
                        tp.DeleteTextInRun(1);
                    }
                    break;
                case Key.Up:

                    var previousRun = GetPreviousRun(_focusedInline);
                    if (previousRun != null)
                    {
                        inputField.Text = previousRun.Text;
                        _focusedInline = previousRun;
                    }
                    break;
                case Key.Down:

                    var nextRun = GetNextRun(_focusedInline);
                    if (nextRun != null)
                    {
                        inputField.Text = nextRun.Text;
                        _focusedInline = nextRun;
                    }
                    break;
            }
            ScrollToEnd();
        }
        #endregion


        #region action methods

        private void pasteFromClipboard()
        {
            if (getPositionType(inputField) != CuretPositionType.Outside)
                CaretPosition.InsertTextInRun(Clipboard.GetText());//Inserting text
            else
                inputField.ContentEnd.InsertTextInRun(Clipboard.GetText());
        }

        #endregion

        #region Positioning Tools
        CustomRun.CustomType previousStyle = CustomRun.CustomType.Red;
        private CustomRun.CustomType getNextStyle()
        {
            if (previousStyle == CustomRun.CustomType.Blue)
            {
                previousStyle = CustomRun.CustomType.Red;
            }
            else
            {
                previousStyle = CustomRun.CustomType.Blue;
            }
            return previousStyle;
        }


        private enum CuretPositionType
        {
            Start,
            End,
            Middle,
            Outside
        }

        private CuretPositionType getPositionType(Run relativeTarget)
        {
            //     1  |   cursor   |  -1
            //     cursor == start  -> start == 0  end == 1
            //     cursor == end    -> start == 1  end == 0
            int start = CaretPosition.CompareTo(relativeTarget.ContentStart);
            int end = CaretPosition.CompareTo(relativeTarget.ContentEnd);
            if (start == 1 && end == -1)
                return CuretPositionType.Middle;
            if (start == 0)
                return CuretPositionType.Start;
            if (end == 0)
                return CuretPositionType.End;
            return CuretPositionType.Outside;
        }

        private TextRange GetSelection(Inline element)
        {
            int pos1 = Selection.Start.CompareTo(element.ContentStart);
            int pos2 = element.ContentEnd.CompareTo(Selection.End);
            int pos3 = Selection.End.CompareTo(element.ContentStart);

            if (pos3 == -1 || Selection.IsEmpty)
                return null;


            TextPointer start = Selection.Start;
            TextPointer end = Selection.End;
            if (pos1 < 0)
                start = element.ContentStart;
            if (pos2 < 0)
                end = element.ContentEnd;

            return new TextRange(start, end);
        }

        private Run GetNextRun(Run run)
        {
            var runs = readOnlyItems.Inlines.OfType<CustomRun.CommandRun>();
            if (runs.Count() == 0)
                return CustomRun.Cmd;

            int nextIndex = run != null ? runs.TakeWhile(x => x != run).Count() + 1 : -1;
            if (nextIndex > runs.Count() - 1)
                return runs.ElementAt(runs.Count() - 1);

            nextIndex = nextIndex > 0 ? nextIndex : 0;
            return runs.ElementAt(nextIndex);
        }

        private Run GetPreviousRun(Run run)
        {
            var runs = readOnlyItems.Inlines.OfType<CustomRun.CommandRun>();
            if (runs.Count() == 0)
                return CustomRun.Cmd;

            if (run == null)
                return runs.LastOrDefault();

            int previousIndex = runs.TakeWhile(x => x != run).Count() - 1;
            previousIndex = previousIndex > -1 ? previousIndex : 0;
            return runs.ElementAt(previousIndex);
        }

        
        #endregion

        public bool IsEnabledCustom
        {
            get => isEnabledCustom;
            set
            {
                isEnabledCustom = value;
                OnIsEnabledChanged();
            }
        }

        private void onLoaded(object sender, RoutedEventArgs e)
        {
            OnIsEnabledChanged();
        }

        private void OnIsEnabledChanged()
        {
            if (!IsLoaded)
                return;


            if (IsEnabledCustom)
            {
                subscribeEditableElement();
                Focus();
            }
            else
            {
                // delete editable element
                inputFieldParent.Inlines.Clear();
                CaretBrush = Brushes.Transparent;
                //CaretPosition = inputField.ContentEnd; ToDo: carret is not on propper place at start
            }
        }

        private void onPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (base.Selection.IsEmpty)
            {
                inputField.Text += Clipboard.GetText();//Paste from clipboard
                CaretPosition = inputField.ContentEnd;
                ScrollToEnd();
            }
            else
            {
                Clipboard.SetText(base.Selection.Text, TextDataFormat.Text);//Copy to Clipboard
                CaretPosition = Selection.End;//deselect
            }
        }

        public void Clear()
        {
            readOnlyItems.Dispatcher.BeginInvoke(new Action(() =>
            {
                readOnlyItems.Inlines.Clear();
                inputField.Text = "";
                _focusedInline = null;
                Focus();
            }));
        }

        public string GetText()
        {
            TextRange textRange = new TextRange(
                // TextPointer to the start of content in the RichTextBox.
                Document.ContentStart,
                // TextPointer to the end of content in the RichTextBox.
                Document.ContentEnd
            );

            // The Text property on a TextRange object returns a string 
            // representing the plain text content of the TextRange. 
            return textRange.Text;
        }
        object lockObject = new object();

        public void SetText(string message, RunType runType)
        {
            readOnlyItems.Dispatcher.BeginInvoke(new Action(() =>
            {
                var run = runFactory.Get(runType) as Run;
                run.Text = message; // + Environment.NewLine;
                readOnlyItems.Inlines.Add(run);
                readOnlyItems.Inlines.Add(new Run(Environment.NewLine));//problems with extra lines
                ScrollToEnd();
                _focusedInline = null;
            }
            ));
        }

        public static class ThreadContext
        {
            public static void InvokeOnUiThread(Action action)
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    action();
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(action);
                }
            }

            public static void BeginInvokeOnUiThread(Action action)
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    action();
                }
                else
                {
                    Application.Current.Dispatcher.BeginInvoke(action);
                }
            }
        }

        //private void MoveCustomCaret()
        //{

        //    var caretLocation = CaretPosition.GetCharacterRect(LogicalDirection.Backward).Location;

        //    if (!double.IsInfinity(caretLocation.X))
        //    {
        //        Canvas.SetLeft(Caret, 0);
        //    }

        //    if (!double.IsInfinity(caretLocation.Y))
        //    {
        //        Canvas.SetTop(Caret, -10);
        //    }
        //}

        /// <param name="category">Info - message from device only, Debug - messages from my app like "Successfull"</param>
        //public void Log(string message, Category category, Priority priority)
        //{
        //    readOnlyItems.Dispatcher.BeginInvoke(new Action(() =>
        //    {

        //        CustomRun run = CustomRun.Log;

        //        switch (category)
        //        {
        //            case Category.Exception:
        //                run = CustomRun.Error;
        //                break;
        //            case Category.Debug:
        //                run = CustomRun.Debug;
        //                break;
        //            case Category.Info:
        //                run = CustomRun.Info;//something I do not control
        //                break;
        //                //case Category.Warn:
        //                //    run = CustomRun.Cmd;
        //                //    break;
        //        }
        //        run.Text = message;
        //        //if (category != Category.Info)
        //        //{
        //        run.Text = Environment.NewLine + message;
        //        //}
        //        readOnlyItems.Inlines.Add(run);
        //        ScrollToEnd();
        //        _focusedInline = null;
        //    }));

        //}
    }
}
