namespace Epoche.MVVM.ViewModels;

public interface INotificationService
{
    void ShowMessage(NotificationType type, string message);
}
