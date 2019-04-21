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
        public int Latitude { get; set; }
        public int Longitude { get; set; }

        public virtual ICollection<ArticleGpsTag> ArticlesGpsTags { get; set; }
    }
}
