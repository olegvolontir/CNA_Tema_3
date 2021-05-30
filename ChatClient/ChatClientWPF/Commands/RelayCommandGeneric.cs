using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatClientWPF.Commands
{
    class RelayCommandGeneric<T> : ICommand
    {
        private Action<T> commandTask;
        private Predicate<T> canExecuteTask;

        public RelayCommandGeneric(Action<T> workToDo, Predicate<T> canExecute)
        {
            commandTask = workToDo;
            canExecuteTask = canExecute;
        }

        public RelayCommandGeneric(Action<T> workToDo)
            : this(workToDo, DefaultCanExecute)
        {
            commandTask = workToDo;
        }

        private static bool DefaultCanExecute(T parameter)
        {
            return true;
        }

        public bool CanExecute(object parameter)
        {
            return canExecuteTask != null && canExecuteTask((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public void Execute(object parameter)
        {
            commandTask((T)parameter);
        }
    }
}
