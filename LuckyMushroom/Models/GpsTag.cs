using System.Collections.Generic;

namespace LuckyMushroom.Models
{
    public class GpsTag
    {
        public GpsTag()
        {
            ArticlesGpsTags = new HashSet<ArticleGpsTag>();
        }

        public int TagId { get; set; }
        public int SecondsNorth { get; set; }
        public int SecondsWest { get; set; }

        public virtual ICollection<ArticleGpsTag> ArticlesGpsTags { get; set; }
    }
}
