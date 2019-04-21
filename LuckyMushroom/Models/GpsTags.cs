using System;
using System.Collections.Generic;

namespace LuckyMushroom.Models
{
    public partial class GpsTags
    {
        public GpsTags()
        {
            ArticlesGpsTags = new HashSet<ArticlesGpsTags>();
        }

        public int TagId { get; set; }
        public int SecondsNorth { get; set; }
        public int SecondsWest { get; set; }

        public virtual ICollection<ArticlesGpsTags> ArticlesGpsTags { get; set; }
    }
}
