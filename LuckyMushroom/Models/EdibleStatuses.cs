using System;
using System.Collections.Generic;

namespace LuckyMushroom.Models
{
    public partial class EdibleStatuses
    {
        public EdibleStatuses()
        {
            RecognitionRequests = new HashSet<RecognitionRequests>();
        }

        public string EdibleStatusAlias { get; set; }
        public string EdibleDescription { get; set; }
        public int EdibleStatusId { get; set; }

        public virtual ICollection<RecognitionRequests> RecognitionRequests { get; set; }
    }
}
