using System.Collections.Generic;

namespace LuckyMushroom.Models
{
    public partial class Role
    {
        public Role()
        {
            Users = new HashSet<User>();
        }

        public string RoleAlias { get; set; }
        public string RoleName { get; set; }
        public uint RoleId { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
