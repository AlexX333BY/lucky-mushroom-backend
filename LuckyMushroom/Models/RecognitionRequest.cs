using System;
using System.Collections.Generic;

namespace LuckyMushroom.Models
{
    public partial class RecognitionRequest
    {
        public RecognitionRequest()
        {
            RequestPhotos = new HashSet<RequestPhoto>();
        }

        public int RequestId { get; set; }
        public DateTime RequestDatetime { get; set; }
        public int RequesterId { get; set; }
        public int StatusId { get; set; }
        public int? EdibleStatusId { get; set; }

        public virtual EdibleStatus EdibleStatus { get; set; }
        public virtual User Requester { get; set; }
        public virtual RecognitionStatus Status { get; set; }
        public virtual ICollection<RequestPhoto> RequestPhotos { get; set; }
    }
}
