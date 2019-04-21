﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuckyMushroom.Models;

namespace LuckyMushroom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly LuckyMushroomContext _context;
        protected byte MaxLatitude { get; private set; }
        protected byte MaxLongitude { get; private set; }

        public ArticlesController(LuckyMushroomContext context)
        {
            _context = context;
            MaxLatitude = 90;
            MaxLongitude = 180;
        }

        [HttpGet("byGpsTag")]
        public async Task<IActionResult> GetArticle(int latitudeSeconds, int longitudeSeconds)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if ((Math.Abs(latitudeSeconds) > MaxLatitude) || (Math.Abs(longitudeSeconds) > MaxLongitude))
            {
                return BadRequest();
            }

            int maxLatitudeDistance = MaxLatitude * 2, maxLongitudeDistance = MaxLongitude * 2;
            GpsTag nearestGpsTag = (await _context.GpsTags.ToArrayAsync()).OrderBy((tag) =>
            {
                int latitudeDistance = Math.Abs(latitudeSeconds - tag.LatitudeSeconds),
                    longitudeDistance = Math.Abs(longitudeSeconds - tag.LongitudeSeconds);
                int normalizedLatitudeDistance = Math.Min(latitudeDistance, Math.Abs(maxLatitudeDistance - latitudeDistance)),
                    normalizedLongitudeDistance = Math.Min(longitudeDistance, Math.Abs(maxLongitudeDistance - longitudeDistance));
                return Math.Sqrt(Math.Pow(normalizedLatitudeDistance, 2) + Math.Pow(normalizedLongitudeDistance, 2));
            }).First();

            var articles = (await _context.ArticlesGpsTags.Where((agt) => agt.TagId == nearestGpsTag.TagId).ToArrayAsync()).Select((agt) => (agt.ArticleId, agt.Article.ArticleText));

            return Ok(articles);
        }

        [HttpPost("add")]
        public async Task<IActionResult> PostArticle([FromBody] string text, [FromBody] int latitude, [FromBody] int longitude)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if ((Math.Abs(latitude) > MaxLatitude) || (Math.Abs(longitude) > MaxLongitude))
            {
                return BadRequest();
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    uint dbTagId = ((await _context.GpsTags.SingleOrDefaultAsync((tag) => tag.LatitudeSeconds == latitude && tag.LongitudeSeconds == longitude))
                        ?? (await _context.GpsTags.AddAsync(new GpsTag() { LatitudeSeconds = latitude, LongitudeSeconds = longitude })).Entity).TagId;

                    Article newArticle = (await _context.Articles.AddAsync(new Article() { ArticleText = text })).Entity;

                    await _context.ArticlesGpsTags.AddAsync(new ArticleGpsTag { ArticleId = newArticle.ArticleId, TagId = dbTagId });
                    await _context.SaveChangesAsync();

                    transaction.Commit();

                    return Ok((newArticle.ArticleId, newArticle.ArticleText));
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteArticle(uint id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
