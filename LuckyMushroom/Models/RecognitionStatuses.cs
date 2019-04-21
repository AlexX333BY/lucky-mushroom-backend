using System;
using System.Collections.Generic;

namespace LuckyMushroom.Models
{
    public partial class RecognitionStatuses
    {
        public RecognitionStatuses()
        {
            RecognitionRequests = new HashSet<RecognitionRequests>();
        }

        public string StatusAlias { get; set; }
        public string StatusName { get; set; }
        public int StatusId { get; set; }

        public virtual ICollection<RecognitionRequests> RecognitionRequests { get; set; }
    }
}
