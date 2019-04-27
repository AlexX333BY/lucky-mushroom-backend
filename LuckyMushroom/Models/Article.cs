using System.Collections.Generic;

namespace LuckyMushroom.Models
{
    public partial class Article
    {
        public Article()
        {
            ArticlesGpsTags = new HashSet<ArticleGpsTag>();
        }

        public int ArticleId { get; set; }
        public string ArticleText { get; set; }

        public virtual ICollection<ArticleGpsTag> ArticlesGpsTags { get; set; }
    }
}
