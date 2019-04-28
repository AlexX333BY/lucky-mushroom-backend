using System.IO;
using LuckyMushroom.Models;
using LuckyMushroom.Helpers;

namespace LuckyMushroom.DataTransferObjects
{
    public class RequestPhotoDto
    {
        public RequestPhotoDto(RequestPhoto photo)
        {
            PhotoId = photo?.PhotoId;
            if (photo?.PhotoFilename != null)
            {
                PhotoExtension = Path.GetExtension(photo.PhotoFilename);
                PhotoData = new RequestPhotoSaver().ReadPhoto(photo.PhotoFilename);
            }
        }
        public int? PhotoId { get; set; }
        public string PhotoExtension { get; set; }
        public byte[] PhotoData { get; set; }
    }
}
