using System.Collections.Generic;

namespace LuckyMushroom.Models
{
    public partial class RecognitionStatus
    {
        public RecognitionStatus()
        {
            RecognitionRequests = new HashSet<RecognitionRequest>();
        }

        public string StatusAlias { get; set; }
        public string StatusName { get; set; }
        public int StatusId { get; set; }

        public virtual ICollection<RecognitionRequest> RecognitionRequests { get; set; }
    }
}
