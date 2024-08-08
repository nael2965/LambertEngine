using System.Windows.Input;

namespace LambertEditor.Common;

class RelayCommand<T> : ICommand
{
    private readonly Action<T> _excute;
    private readonly Predicate<T> _canExcute;
    
    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public bool CanExecute(object parameter)
    {
        return _canExcute?.Invoke((T)parameter) ?? true;
    }
    
    public void Execute(object parameter)
    {
        _excute((T)parameter);
    }

    public RelayCommand(Action<T> execute, Predicate<T> canExcute = null)
    {
        _excute = execute ?? throw new ArgumentException(nameof(execute));
        _canExcute = canExcute;
    }
}