using System.Collections.Generic;

namespace LuckyMushroom.Models
{
    public class GpsTag
    {
        public GpsTag()
        {
            ArticlesGpsTags = new HashSet<ArticleGpsTag>();
        }

        public uint TagId { get; set; }
        public uint SecondsNorth { get; set; }
        public uint SecondsWest { get; set; }

        public virtual ICollection<ArticleGpsTag> ArticlesGpsTags { get; set; }
    }
}
