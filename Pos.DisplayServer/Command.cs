using System;
using System.Windows.Input;

namespace Pos.DisplayServer
{
    public class Command : ICommand
    {
        private readonly Predicate<object> canExecute;
        private readonly Action<object> execute;

        public Command(Predicate<object> canExecute, Action<object> execute)
        {
            this.canExecute = canExecute;
            this.execute = execute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }

        public void ThrowExecuteChanged()
        {
            this.OnCanExecuteChanged();
        }

        protected virtual void OnCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}