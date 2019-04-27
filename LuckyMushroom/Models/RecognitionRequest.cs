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

        public uint RequestId { get; set; }
        public DateTime RequestDatetime { get; set; }
        public uint RequesterId { get; set; }
        public uint StatusId { get; set; }
        public uint? EdibleStatusId { get; set; }

        public virtual EdibleStatus EdibleStatus { get; set; }
        public virtual User Requester { get; set; }
        public virtual RecognitionStatus Status { get; set; }
        public virtual ICollection<RequestPhoto> RequestPhotos { get; set; }
    }
}
