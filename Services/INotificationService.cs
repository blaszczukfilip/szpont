using szpont.Models;

namespace szpont.Services
{
    public interface INotificationService
    {
        Task CreateTopicReservedNotificationAsync(Topic topic, ApplicationUser student);
        Task CreateReservationCancelledNotificationAsync(Topic topic, ApplicationUser student);
        Task CreateReservationAcceptedNotificationAsync(Topic topic, string studentUserId);
        Task CreateReservationRejectedNotificationAsync(Topic topic, string studentUserId);
        Task<List<Notification>> GetUserNotificationsAsync(string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task MarkAsReadAsync(int notificationId, string userId);
        Task MarkAllAsReadAsync(string userId);
    }
}
