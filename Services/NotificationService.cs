using Microsoft.EntityFrameworkCore;
using szpont.Data;
using szpont.Models;

namespace szpont.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateTopicReservedNotificationAsync(Topic topic, ApplicationUser student)
        {
            var notification = new Notification
            {
                UserId = topic.PromotorId,
                Type = NotificationType.TopicReserved,
                Message = $"Student {student.FirstName} {student.LastName} ({student.StudentIndex}) zarezerwował temat \"{topic.Title}\".",
                TopicId = topic.Id,
                StudentId = student.Id,
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(string userId)
        {
            return await _context.Notifications
                .Include(n => n.Topic)
                .Include(n => n.Student)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();
        }

        public async Task MarkAsReadAsync(int notificationId, string userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }
        public async Task CreateReservationCancelledNotificationAsync(Topic topic, ApplicationUser student)
        {
            var notification = new Notification
            {
                UserId = topic.PromotorId,
                Type = NotificationType.ReservationCancelled,
                Message = $"Student {student.FirstName} {student.LastName} ({student.StudentIndex}) anulował rezerwację tematu \"{topic.Title}\".",
                TopicId = topic.Id,
                StudentId = student.Id,
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }
    }
}
