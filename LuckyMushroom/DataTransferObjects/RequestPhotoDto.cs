using System.IO;
using LuckyMushroom.Models;

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
                PhotoData = File.ReadAllBytes(photo.PhotoFilename);
            }
        }
        public int? PhotoId { get; set; }
        public string PhotoExtension { get; set; }
        public byte[] PhotoData { get; set; }
    }
}
