using LuckyMushroom.Models;
using System.Collections.Generic;
using System.Linq;

namespace LuckyMushroom.DataTransferObjects
{
    public partial class ArticleDto
    {
        public ArticleDto(Article article)
        {
            ArticleText = article?.ArticleText;
            GpsTags = article?.ArticlesGpsTags?.Select((agt) => new GpsTagDto(agt.Tag)).ToArray();
            ArticleId = article?.ArticleId;
        }

        public string ArticleText { get; set; }
        public int? ArticleId { get; set; }
        public GpsTagDto[] GpsTags { get; set; }
    }
}
