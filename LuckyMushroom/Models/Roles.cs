using System;
using System.Collections.Generic;

namespace LuckyMushroom.Models
{
    public partial class Roles
    {
        public Roles()
        {
            Users = new HashSet<Users>();
        }

        public string RoleAlias { get; set; }
        public string RoleName { get; set; }
        public int RoleId { get; set; }

        public virtual ICollection<Users> Users { get; set; }
    }
}
