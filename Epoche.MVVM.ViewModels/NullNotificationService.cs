using System.Diagnostics;

namespace Epoche.MVVM.ViewModels;

sealed class NullNotificationService : INotificationService
{
    public void ShowMessage(NotificationType type, string message) => Debug.WriteLine($"Type: {type} Message: {message}");
}
