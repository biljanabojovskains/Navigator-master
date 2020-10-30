using System;
using Navigator.Models.Abstract;

namespace Navigator.Models.Concrete
{
    [Serializable]
    public class User : IUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
        public Role UserRole { get; set; }
    }
}