using System;
using System.Collections.Generic;

namespace LuckyMushroom.Models
{
    public partial class Articles
    {
        public Articles()
        {
            ArticlesGpsTags = new HashSet<ArticlesGpsTags>();
        }

        public int ArticleId { get; set; }
        public string ArticleText { get; set; }

        public virtual ICollection<ArticlesGpsTags> ArticlesGpsTags { get; set; }
    }
}
