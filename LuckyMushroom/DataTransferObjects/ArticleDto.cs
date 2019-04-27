using LuckyMushroom.Models;
using System.Collections.Generic;
using System.Linq;

namespace LuckyMushroom.DataTransferObjects
{
    public class ArticleDto
    {
        public ArticleDto(Article article)
        {
            ArticleText = article?.ArticleText;
            GpsTags = article?.ArticlesGpsTags?.Select((agt) => new GpsTagDto(agt.Tag)).ToArray();
        }

        public string ArticleText { get; protected set; }
        public GpsTagDto[] GpsTags { get; protected set; }
    }
}
