namespace Epoche.MVVM.Models;

public interface INotifyTask
{
    bool IsCompleted { get; }
    bool IsNotCompleted { get; }
    bool IsSuccessfullyCompleted { get; }
    bool IsUnsuccessfullyCompleted { get; }
    /// <summary>
    /// This wrapper task will complete successfully when the wrapped task completes/cancels/throws.
    /// This task will never throw.
    /// </summary>
    Task TaskCompletion { get; }
}
