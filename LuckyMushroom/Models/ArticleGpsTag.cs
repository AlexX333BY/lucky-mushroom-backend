namespace LuckyMushroom.Models
{
    public class ArticleGpsTag
    {
        public int TagId { get; set; }
        public int ArticleId { get; set; }

        public virtual Article Article { get; set; }
        public virtual GpsTag Tag { get; set; }
    }
}
