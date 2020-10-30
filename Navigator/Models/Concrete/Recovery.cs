using System;
using Navigator.Models.Abstract;

namespace Navigator.Models.Concrete
{
    public class Recovery : IRecovery
    {
        public int RecoveryId { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }

        public DateTime ValidThrough { get; set; }
    }
}