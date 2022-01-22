using System.Windows.Input;

namespace Epoche.MVVM.ViewModels;
public class DelegateCommand : ICommand
{
    public event EventHandler? CanExecuteChanged;
    readonly Action ExecuteFunc;
    readonly Func<bool>? CanExecuteFunc;

    public DelegateCommand(Action executeFunc)
    {
        ExecuteFunc = executeFunc ?? throw new ArgumentNullException(nameof(executeFunc));
    }
    public DelegateCommand(Action executeFunc, Func<bool>? canExecuteFunc) : this(executeFunc)
    {
        CanExecuteFunc = canExecuteFunc;
    }

    public virtual bool CanExecute(object? parameter) => CanExecuteFunc?.Invoke() ?? true;
    public virtual void Execute(object? parameter) => ExecuteFunc();
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
public class DelegateCommand<T> : DelegateCommand
{
    static void Dummy() { }

    readonly Action<T> ExecuteFunc;
    readonly Func<T, bool>? CanExecuteFunc;

    public DelegateCommand(Action<T> executeFunc) : base(Dummy)
    {
        ExecuteFunc = executeFunc ?? throw new ArgumentNullException(nameof(executeFunc));
    }
    public DelegateCommand(Action<T> executeFunc, Func<T, bool>? canExecuteFunc) : this(executeFunc)
    {
        CanExecuteFunc = canExecuteFunc;
    }

    public override bool CanExecute(object? parameter) => CanExecuteFunc?.Invoke((T)parameter!) ?? true;
    public override void Execute(object? parameter) => ExecuteFunc((T)parameter!);
}
