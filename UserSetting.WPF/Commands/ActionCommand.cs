using System.Windows.Input;

namespace UserSetting.WPF.Commands;

internal class ActionCommand(Action<object> action, Predicate<object>? predicate = default) : ICommand
{
    private readonly Action<object> action = action ?? throw new ArgumentNullException(nameof(action), "You must specify an Action<T>.");

    private readonly Predicate<object> predicate = predicate!;

    #region Implementation of ICommand

    public bool CanExecute(object? parameter)
    {
        return predicate == default || predicate(parameter!);
    }

    public void Execute(object? parameter)
    {
        action(parameter!);
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    #endregion Implementation of ICommand
}
