using System;
using System.Windows.Input;

namespace MoveFilesUpOneLevel.ServiceClasses
{
    public class DelegateCommand : ICommand
    {
        Func<object, bool> canExecute;
        Action<object> executeAction;

        public DelegateCommand(Action<object> executeAction)
            : this(executeAction, null)        {        }

        public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecute)
        {
            if (executeAction == null)
            {
                throw new ArgumentNullException("executeAction");
            }
            this.executeAction = executeAction;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter) //can command execute in its current status?
        {
            bool result = true;
            Func<object, bool> canExecuteHandler = this.canExecute;
            if (canExecuteHandler != null)
            {
                result = canExecuteHandler(parameter);
            }
            return result;
        }

        public event EventHandler CanExecuteChanged; //occurs when changes occur that affect whether or not the command should execute

        public void RaiseCanExecuteChanged()
        {
            EventHandler handler = this.CanExecuteChanged;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        public void Execute(object parameter) //Method to call when the command is invoked
        {
            this.executeAction(parameter);
        }
    }
}
