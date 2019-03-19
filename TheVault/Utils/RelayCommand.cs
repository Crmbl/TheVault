using System;
using System.Windows.Input;

namespace TheVault.Utils
{
    public class RelayCommand : ICommand
    {
        private readonly bool _canExecute;
        private readonly Action<object> _execute;

        public RelayCommand(bool canExecute, Action<object> execute)
        {
            _canExecute = canExecute;
            _execute = execute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
