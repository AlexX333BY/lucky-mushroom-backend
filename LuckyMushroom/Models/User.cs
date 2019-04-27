using System.Collections.Generic;

namespace LuckyMushroom.Models
{
    public partial class User
    {
        public User()
        {
            RecognitionRequests = new HashSet<RecognitionRequest>();
        }

        public int UserId { get; set; }
        public int RoleId { get; set; }

        public virtual Role Role { get; set; }
        public virtual UserCredentials UserCredentials { get; set; }
        public virtual ICollection<RecognitionRequest> RecognitionRequests { get; set; }
    }
}
