using Notification.Models.Abstract;

namespace Notification.Models.Concrete
{
    public class SentNotification : ISentNotification
    {
        public SentNotification()
        {

        }

        public SentNotification(int tockaId, int izvestuvanjeId)
        {
            TockaId = tockaId;
            IzvestuvanjeId = izvestuvanjeId;
        }

        public int TockaId { get; set; }

        public int IzvestuvanjeId { get; set; }

        public override bool Equals(object other)
        {
            if (!(other is SentNotification))
            {
                return false;
            }
            return IzvestuvanjeId == ((SentNotification) other).IzvestuvanjeId &&
                   TockaId == ((SentNotification) other).TockaId;
        }
    }
}
