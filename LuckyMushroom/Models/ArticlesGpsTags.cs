using System;
using System.Collections.Generic;

namespace LuckyMushroom.Models
{
    public partial class ArticlesGpsTags
    {
        public int TagId { get; set; }
        public int ArticleId { get; set; }

        public virtual Articles Article { get; set; }
        public virtual GpsTags Tag { get; set; }
    }
}
