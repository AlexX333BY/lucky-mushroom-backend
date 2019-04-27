using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuckyMushroom.Models;
using Microsoft.AspNetCore.Authorization;
using LuckyMushroom.DataTransferObjects;

namespace LuckyMushroom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ArticlesController : ControllerBase
    {
        private readonly LuckyMushroomContext _context;
        protected int MaxLatitudeSeconds { get; private set; }
        protected int MaxLongitudeSeconds { get; private set; }

        public ArticlesController(LuckyMushroomContext context)
        {
            _context = context;
            MaxLatitudeSeconds = 90 * 60 * 60;
            MaxLongitudeSeconds = 180 * 60 * 60;
        }

        protected int GetNormalizedLatitudeDistance(int firstLatitudeSeconds, int secondLatitudeSeconds)
        {
            int maxLatitudeDistance = MaxLatitudeSeconds * 2;
            int latitudeDistance = Math.Abs(firstLatitudeSeconds - secondLatitudeSeconds);
            return Math.Min(latitudeDistance, Math.Abs(maxLatitudeDistance - latitudeDistance));
        }

        protected int GetNormalizedLongitudeDistance(int firstLongitudeSeconds, int secondLongitudeSeconds)
        {
            int maxLongitudeDistance = MaxLongitudeSeconds * 2;
            int longitudeDistance = Math.Abs(firstLongitudeSeconds - secondLongitudeSeconds);
            return Math.Min(longitudeDistance, Math.Abs(maxLongitudeDistance - longitudeDistance));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetArticle(int latitudeSeconds, int longitudeSeconds)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if ((Math.Abs(latitudeSeconds) > MaxLatitudeSeconds) || (Math.Abs(longitudeSeconds) > MaxLongitudeSeconds))
            {
                return BadRequest("Not existing place");
            }

            GpsTag nearestGpsTag = _context.GpsTags
                .OrderBy((tag) => Math.Sqrt(Math.Pow(GetNormalizedLatitudeDistance(latitudeSeconds, tag.LatitudeSeconds), 2) + Math.Pow(GetNormalizedLongitudeDistance(longitudeSeconds, tag.LongitudeSeconds), 2)))
                .FirstOrDefault();

            var articles = nearestGpsTag == null ? null : _context.ArticlesGpsTags.Where((agt) => agt.TagId == nearestGpsTag.TagId).Select((agt) => new ArticleDto(agt.Article));
            return Ok(articles == null ? null : await articles.ToArrayAsync());
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddArticle([FromBody] ArticleGpsTag articleTag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!User.IsInRole("admin"))
            {
                return Forbid();
            }

            int latitude = articleTag.Tag.LatitudeSeconds, longitude = articleTag.Tag.LongitudeSeconds;
            string articleText = articleTag.Article.ArticleText;

            if (articleText == null)
            {
                return BadRequest("You should specify article text");
            }

            if ((Math.Abs(latitude) > MaxLatitudeSeconds) || (Math.Abs(longitude) > MaxLongitudeSeconds))
            {
                return BadRequest("Not existing place");
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                int dbTagId = ((await _context.GpsTags.SingleOrDefaultAsync((tag) => tag.LatitudeSeconds == latitude && tag.LongitudeSeconds == longitude))
                    ?? (await _context.GpsTags.AddAsync(new GpsTag() { LatitudeSeconds = latitude, LongitudeSeconds = longitude })).Entity).TagId;

                Article newArticle = (await _context.Articles.AddAsync(new Article() { ArticleText = articleText })).Entity;

                await _context.ArticlesGpsTags.AddAsync(new ArticleGpsTag { ArticleId = newArticle.ArticleId, TagId = dbTagId });
                await _context.SaveChangesAsync();

                transaction.Commit();

                return Ok(newArticle);
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteArticle(uint id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!User.IsInRole("admin"))
            {
                return Forbid();
            }

            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();

            return Ok(article);
        }
    }
}
