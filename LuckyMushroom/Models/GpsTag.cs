using System.Collections.Generic;

namespace LuckyMushroom.Models
{
    public partial class GpsTag
    {
        public GpsTag()
        {
            ArticlesGpsTags = new HashSet<ArticleGpsTag>();
        }

        public int TagId { get; set; }
        public int LatitudeSeconds { get; set; }
        public int LongitudeSeconds { get; set; }

        public virtual ICollection<ArticleGpsTag> ArticlesGpsTags { get; set; }
    }
}
