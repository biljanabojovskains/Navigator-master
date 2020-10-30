using System;
using Notification.Models.Abstract;

namespace Notification.Models.Concrete
{
    public class Notifications : INotifications
    {
        public int TockaId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Text { get; set; }
        public DateTime DateOd { get; set; }
        public DateTime DateDo { get; set; }
        public string Tema { get; set; }
        public string Podtema { get; set; }
        public int IzvestuvanjeId { get; set; }
    }
}
