using System.Collections.Generic;

namespace LuckyMushroom.Models
{
    public class User
    {
        public User()
        {
            RecognitionRequests = new HashSet<RecognitionRequest>();
        }

        public uint UserId { get; set; }
        public uint RoleId { get; set; }

        public virtual Role Role { get; set; }
        public virtual UserCredentials UserCredentials { get; set; }
        public virtual ICollection<RecognitionRequest> RecognitionRequests { get; set; }
    }
}
