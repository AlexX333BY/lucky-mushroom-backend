using LuckyMushroom.Models;

namespace LuckyMushroom.DataTransferObjects
{
    public class RecognitionStatusDto
    {
        public RecognitionStatusDto(RecognitionStatus status)
        {
            RecognitionStatusAlias = status?.StatusAlias;
            RecognitionStatusName = status?.StatusName;
            RecognitionStatusId = status?.StatusId;
        }

        public string RecognitionStatusAlias { get; set; }
        public string RecognitionStatusName { get; set; }
        public int? RecognitionStatusId { get; set; }
    }
}
