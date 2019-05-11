using System.Linq;
using LuckyMushroom.Models;

namespace LuckyMushroom.DataTransferObjects
{
    public class RecognitionRequestWithMultiplePhotosDto : RecognitionRequestDtoBase
    {
        public RecognitionRequestWithMultiplePhotosDto(RecognitionRequest request) : base(request)
        {
            RequestPhotos = request?.RequestPhotos?.Select((photo) => new RequestPhotoDto(photo)).ToArray();
        }

        public RequestPhotoDto[] RequestPhotos { get; set; }
    }
}
