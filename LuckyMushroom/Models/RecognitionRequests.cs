using System;
using System.Collections.Generic;

namespace LuckyMushroom.Models
{
    public partial class RecognitionRequests
    {
        public RecognitionRequests()
        {
            RequestPhotos = new HashSet<RequestPhotos>();
        }

        public int RequestId { get; set; }
        public DateTime RequestDatetime { get; set; }
        public int RequesterId { get; set; }
        public int StatusId { get; set; }
        public int? EdibleStatusId { get; set; }

        public virtual EdibleStatuses EdibleStatus { get; set; }
        public virtual Users Requester { get; set; }
        public virtual RecognitionStatuses Status { get; set; }
        public virtual ICollection<RequestPhotos> RequestPhotos { get; set; }
    }
}
