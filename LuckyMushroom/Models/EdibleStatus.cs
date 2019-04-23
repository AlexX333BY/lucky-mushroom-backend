﻿using System.Collections.Generic;

namespace LuckyMushroom.Models
{
    public class EdibleStatus
    {
        public EdibleStatus()
        {
            RecognitionRequests = new HashSet<RecognitionRequest>();
        }

        public string EdibleStatusAlias { get; set; }
        public string EdibleDescription { get; set; }
        public uint EdibleStatusId { get; set; }

        public virtual ICollection<RecognitionRequest> RecognitionRequests { get; set; }
    }
}