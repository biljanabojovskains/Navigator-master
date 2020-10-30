using System.Collections.Generic;
using Notification.Models.Abstract;

namespace Notification.Dal.Abstract
{
    public interface INotificationDal
    {
        List<INotifications> GetAll();
        List<ISentNotification> GetAllSent();
    }
}
