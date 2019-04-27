using LuckyMushroom.Models;

namespace LuckyMushroom.DataTransferObjects
{
    public class GpsTagDto
    {
        public GpsTagDto(GpsTag tag)
        {
            LatitudeSeconds = tag?.LatitudeSeconds;
            LongitudeSeconds = tag?.LongitudeSeconds;
        }

        public int? LatitudeSeconds { get; protected set; }
        public int? LongitudeSeconds { get; protected set; }
    }
}
