using LuckyMushroom.Models;

namespace LuckyMushroom.DataTransferObjects
{
    public class EdibleStatusDto
    {
        public EdibleStatusDto(EdibleStatus status)
        {
            EdibleDescription = status?.EdibleDescription;
            EdibleStatusAlias = status?.EdibleStatusAlias;
            EdibleStatusId = status?.EdibleStatusId;
        }

        public string EdibleStatusAlias { get; set; }
        public string EdibleDescription { get; set; }
        public int? EdibleStatusId { get; set; }
    }
}
