using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuckyMushroom.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;
using LuckyMushroom.DataTransferObjects;

namespace LuckyMushroom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
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

            GpsTag nearestGpsTag = await _context.GpsTags
                .OrderBy((tag) => Math.Sqrt(Math.Pow(GetNormalizedLatitudeDistance(latitudeSeconds, tag.LatitudeSeconds), 2) + Math.Pow(GetNormalizedLongitudeDistance(longitudeSeconds, tag.LongitudeSeconds), 2)))
                .FirstOrDefaultAsync();

            var articles = nearestGpsTag == null ? null : _context.ArticlesGpsTags.Where((agt) => agt.TagId == nearestGpsTag.TagId).Select((agt) => new ArticleDto(agt.Article));
            return Ok(articles == null ? Array.Empty<ArticleDto>() : await articles.ToArrayAsync());
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddArticle([FromBody] ArticleDto article)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!User.IsInRole("admin"))
            {
                return Forbid();
            }

            string articleText = article.ArticleText;

            if (articleText == null)
            {
                return BadRequest("You should specify article text");
            }

            string articleTitle = article.ArticleTitle;

            if (articleTitle == null)
            {
                return BadRequest("You should specify article title");
            }

            if ((article.GpsTags == null) || (article.GpsTags.Length == 0))
            {
                return BadRequest("Article should specify at least one place");
            }

            foreach (GpsTagDto tag in article.GpsTags)
            {
                if (!tag.LatitudeSeconds.HasValue || !tag.LongitudeSeconds.HasValue)
                {
                    return BadRequest(("Place in unspecified", tag));
                }
                if ((Math.Abs(tag.LatitudeSeconds.Value) > MaxLatitudeSeconds) || (Math.Abs(tag.LongitudeSeconds.Value) > MaxLongitudeSeconds))
                {
                    return BadRequest(("Not existing place", tag));
                }
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                var newArticle = (await _context.Articles.AddAsync(new Article { ArticleText = articleText, ArticleTitle = articleTitle })).Entity;

                List<Task> articleTagAddTasks = new List<Task>(article.GpsTags.Length); 
                foreach (GpsTagDto tagDto in article.GpsTags)
                {
                    int dbTagId = (await _context.GpsTags.SingleOrDefaultAsync((tag) => (tag.LatitudeSeconds == tagDto.LatitudeSeconds.Value) && (tag.LongitudeSeconds == tagDto.LongitudeSeconds.Value)) 
                        ?? (await _context.GpsTags.AddAsync(new GpsTag { LatitudeSeconds = tagDto.LatitudeSeconds.Value, LongitudeSeconds = tagDto.LongitudeSeconds.Value })).Entity).TagId;

                    articleTagAddTasks.Add(_context.AddAsync(new ArticleGpsTag { ArticleId = newArticle.ArticleId, TagId = dbTagId }));
                }

                await Task.WhenAll(articleTagAddTasks);
                await _context.SaveChangesAsync();

                transaction.Commit();

                return Ok(new ArticleDto(newArticle));
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteArticle(int id)
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

            return Ok(new ArticleDto(article));
        }
    }
}
