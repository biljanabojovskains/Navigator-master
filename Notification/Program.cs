using Notification.Dal.Concrete;
using Notification.Models.Concrete;

namespace Notification
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var notifications = new NotificationDa().GetAll();
            var sentNotifications = new NotificationDa().GetAllSent();
            foreach (var item in notifications)
            {
                var exists = sentNotifications.Contains(new SentNotification(item.TockaId, item.IzvestuvanjeId));
                if (!exists)
                {
                    var result = Mail.SendEmail(item.FullName, item.DateOd, item.DateDo, item.Email, item.Text,
                        item.Tema, item.Podtema);
                    if (result)
                    {
                        new NotificationDa().Update(item.TockaId, item.IzvestuvanjeId);
                    }
                }
            }
        }
    }
}
