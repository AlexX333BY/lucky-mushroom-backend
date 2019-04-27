namespace LuckyMushroom.Models
{
    public partial class ArticleGpsTag
    {
        public uint TagId { get; set; }
        public uint ArticleId { get; set; }

        public virtual Article Article { get; set; }
        public virtual GpsTag Tag { get; set; }
    }
}
