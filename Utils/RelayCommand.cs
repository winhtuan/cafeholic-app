using System;
using System.Windows.Input;

namespace CAFEHOLIC.Utils
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> execute;
        private readonly Predicate<T>? canExecute;

        public RelayCommand(Action<T> execute, Predicate<T>? canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) =>
            canExecute == null || parameter is T t && canExecute(t) || parameter == null;

        public void Execute(object? parameter)
        {
            System.Diagnostics.Debug.WriteLine($"[RelayCommand] Executing with parameter: {parameter?.ToString() ?? "null"}");
            if (parameter is T t || parameter == null)
            {
                System.Diagnostics.Debug.WriteLine($"[RelayCommand] Parameter cast to {typeof(T).Name} successfully or is null");
                execute((T)parameter!); // Ép kiểu null thành T nếu parameter là null
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[RelayCommand] Parameter is not of type {typeof(T).Name}, parameter type: {(parameter != null ? parameter.GetType().Name : "null")}");
            }
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> execute;
        private readonly Func<bool>? canExecute;
        private bool isExecuting;

        public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) =>
            !isExecuting && (canExecute?.Invoke() ?? true);

        public async void Execute(object? parameter)
        {
            try
            {
                isExecuting = true;
                RaiseCanExecuteChanged();
                await execute();
            }
            finally
            {
                isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void RaiseCanExecuteChanged() =>
            CommandManager.InvalidateRequerySuggested();
    }
}