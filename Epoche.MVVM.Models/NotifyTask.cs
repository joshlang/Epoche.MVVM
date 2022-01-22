namespace Epoche.MVVM.Models;

// Thanks Stephen Cleary :)
public sealed class NotifyTask<TResult> : ModelBase, INotifyTask
{
    static readonly PropertyChangedEventArgs[] CompletePropertyChanges = new[]
    {
        new PropertyChangedEventArgs(nameof(Result)),
        new PropertyChangedEventArgs(nameof(IsCompleted)),
        new PropertyChangedEventArgs(nameof(IsNotCompleted)),
        new PropertyChangedEventArgs(nameof(IsSuccessfullyCompleted)),
        new PropertyChangedEventArgs(nameof(IsUnsuccessfullyCompleted))
    };

    public NotifyTask(Task<TResult> task)
    {
        Task = task ?? throw new ArgumentNullException(nameof(task));

        TaskCompletion = task.IsCompleted ? System.Threading.Tasks.Task.CompletedTask : Notify();
    }

    async Task Notify()
    {
        try
        {
            await Task.ConfigureAwait(false);
        }
        catch { }
        RaisePropertiesChanged(CompletePropertyChanges);
    }

    /// <summary>
    /// This is the task being watched.
    /// </summary>
    public Task<TResult> Task { get; }
    /// <summary>
    /// This wrapper task will complete successfully when the wrapped task completes/cancels/throws.
    /// This task will never throw.
    /// </summary>
    public Task TaskCompletion { get; }
    /// <summary>
    /// If the task completed successfully, this is the result.
    /// If the task is not complete, or threw an exception, this is null/default.
    /// Accessing this property will never block or throw an exception.
    /// </summary>
    public TResult? Result => Task.Status == TaskStatus.RanToCompletion ? Task.Result : default;
    public bool IsCompleted => Task.IsCompleted;
    public bool IsNotCompleted => !Task.IsCompleted;
    public bool IsSuccessfullyCompleted => Task.IsCompletedSuccessfully;
    public bool IsUnsuccessfullyCompleted => Task.IsCompleted && !Task.IsCompletedSuccessfully;
}

public sealed class NotifyTask : ModelBase, INotifyTask
{
    static readonly PropertyChangedEventArgs[] CompletePropertyChanges = new[]
    {
        new PropertyChangedEventArgs(nameof(IsCompleted)),
        new PropertyChangedEventArgs(nameof(IsNotCompleted)),
        new PropertyChangedEventArgs(nameof(IsSuccessfullyCompleted)),
        new PropertyChangedEventArgs(nameof(IsUnsuccessfullyCompleted))
    };

    public NotifyTask(Task task)
    {
        Task = task ?? throw new ArgumentNullException(nameof(task));

        TaskCompletion = task.IsCompleted ? Task.CompletedTask : Notify();
    }

    async Task Notify()
    {
        try
        {
            await Task.ConfigureAwait(false);
        }
        catch { }
        RaisePropertiesChanged(CompletePropertyChanges);
    }

    /// <summary>
    /// This is the task being watched.
    /// </summary>
    public Task Task { get; }
    /// <summary>
    /// This wrapper task will complete successfully when the wrapped task completes/cancels/throws.
    /// This task will never throw.
    /// </summary>
    public Task TaskCompletion { get; }
    public bool IsCompleted => Task.IsCompleted;
    public bool IsNotCompleted => !Task.IsCompleted;
    public bool IsSuccessfullyCompleted => Task.IsCompletedSuccessfully;
    public bool IsUnsuccessfullyCompleted => Task.IsCompleted && !Task.IsCompletedSuccessfully;
}
