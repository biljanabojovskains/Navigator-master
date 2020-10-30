using System;

namespace Notification.Models.Abstract
{
    public interface INotifications 
    {
        int TockaId { get; set; }
        int IzvestuvanjeId { get; set; }
        string FullName { get; set; }
        string Email { get; set; }
        string Text { get; set; }
        DateTime DateOd { get; set; }
        DateTime DateDo { get; set; }
        string Tema { get; set; }
        string Podtema { get; set; } 
    }
}
