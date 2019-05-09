using System;
using System.Collections.Generic;
using System.Linq;
using LuckyMushroom.Models;

namespace LuckyMushroom.DataTransferObjects
{
    public class RecognitionRequestDto
    {
        public RecognitionRequestDto(RecognitionRequest request)
        {
            RequestId = request?.RequestId;
            RequestDatetime = request?.RequestDatetime;
            EdibleStatus = request?.EdibleStatus == null ? null : new EdibleStatusDto(request?.EdibleStatus);
            RecognitionStatus = new RecognitionStatusDto(request?.Status);
            RequestPhotos = request?.RequestPhotos?.Select((photo) => new RequestPhotoDto(photo)).ToArray();
        }

        public int? RequestId { get; set; }
        public DateTime? RequestDatetime { get; set; }

        public EdibleStatusDto EdibleStatus { get; set; }
        public RecognitionStatusDto RecognitionStatus { get; set; }
        public RequestPhotoDto[] RequestPhotos { get; set; }
    }
}
