using System.Linq;
using LuckyMushroom.Models;

namespace LuckyMushroom.DataTransferObjects
{
    public class RecognitionRequestWithSinglePhotoDto : RecognitionRequestDtoBase
    {
        public RecognitionRequestWithSinglePhotoDto(RecognitionRequest request) : base(request)
        {
            RequestPhoto = request?.RequestPhotos?.Select((photo) => new RequestPhotoDto(photo)).FirstOrDefault();
        }

        public RequestPhotoDto RequestPhoto { get; set; }
    }
}
