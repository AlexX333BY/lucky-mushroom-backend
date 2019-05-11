using System;
using LuckyMushroom.Models;

namespace LuckyMushroom.DataTransferObjects
{
    public abstract class RecognitionRequestDtoBase
    {
        public RecognitionRequestDtoBase(RecognitionRequest request)
        {
            RequestId = request?.RequestId;
            RequestDatetime = request?.RequestDatetime;
            EdibleStatus = request?.EdibleStatus == null ? null : new EdibleStatusDto(request?.EdibleStatus);
            RecognitionStatus = new RecognitionStatusDto(request?.Status);
        }

        public int? RequestId { get; set; }
        public DateTime? RequestDatetime { get; set; }

        public EdibleStatusDto EdibleStatus { get; set; }
        public RecognitionStatusDto RecognitionStatus { get; set; }
    }
}
