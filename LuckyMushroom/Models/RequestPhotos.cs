using System;
using System.Collections.Generic;

namespace LuckyMushroom.Models
{
    public partial class RequestPhotos
    {
        public int PhotoId { get; set; }
        public string PhotoFilename { get; set; }
        public int? RequestId { get; set; }

        public virtual RecognitionRequests Request { get; set; }
    }
}
