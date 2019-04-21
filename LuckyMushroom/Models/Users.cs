using System;
using System.Collections.Generic;

namespace LuckyMushroom.Models
{
    public partial class Users
    {
        public Users()
        {
            RecognitionRequests = new HashSet<RecognitionRequests>();
        }

        public int UserId { get; set; }
        public int RoleId { get; set; }

        public virtual Roles Role { get; set; }
        public virtual UserCredentials UserCredentials { get; set; }
        public virtual ICollection<RecognitionRequests> RecognitionRequests { get; set; }
    }
}
