using Aware.Util;

namespace Aware.Notification
{
    public interface INotificationService : IBaseService<Notification>
    {
        NotificationListModel GetAllNotifications();
        NotificationDetailModel GetNotification(int notificationID);
        Notification GetUserNotification(int userID);
    }
}
