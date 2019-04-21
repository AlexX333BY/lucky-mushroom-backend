using System.Collections.Generic;

namespace LuckyMushroom.Models
{
    public class Article
    {
        public Article()
        {
            ArticlesGpsTags = new HashSet<ArticleGpsTag>();
        }

        public uint ArticleId { get; set; }
        public string ArticleText { get; set; }

        public virtual ICollection<ArticleGpsTag> ArticlesGpsTags { get; set; }
    }
}
