using System;
using System.Collections.Generic;

namespace LuckyMushroom.Models
{
    public partial class UserCredentials
    {
        public string UserMail { get; set; }
        public string UserPasswordHash { get; set; }
        public int UserId { get; set; }

        public virtual Users User { get; set; }
    }
}
