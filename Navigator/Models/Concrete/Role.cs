using System;
using Navigator.Models.Abstract;

namespace Navigator.Models.Concrete
{
    [Serializable]
    public class Role : IRole
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleNameMk { get; set; }
    }
}