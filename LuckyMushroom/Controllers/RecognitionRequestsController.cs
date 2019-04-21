using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuckyMushroom.Models;

namespace LuckyMushroom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecognitionRequestsController : ControllerBase
    {
        private readonly LuckyMushroomContext _context;

        public RecognitionRequestsController(LuckyMushroomContext context)
        {
            _context = context;
        }

        protected virtual object ResponsedRequest(RecognitionRequest request)
        {
            /* add photos */
            return (request.RequestId, request.RequestDatetime, request.EdibleStatus.EdibleDescription, request.Status.StatusName);
        }

        // GET: api/RecognitionRequests
        [HttpGet]
        public async Task<IActionResult> GetRecognitionRequests(string edibleStatusAlias = null, string recognitionStatusAlias = null)
        {
            IQueryable<RecognitionRequest> requests = _context.RecognitionRequests;

            if (edibleStatusAlias != null)
            {
                EdibleStatus edibleStatus = await _context.EdibleStatuses.Where((status) => status.EdibleStatusAlias == edibleStatusAlias).FirstOrDefaultAsync();

                if (edibleStatus != null)
                {
                    requests = requests.Where((request) => request.EdibleStatusId == edibleStatus.EdibleStatusId);
                }
                else
                {
                    return NotFound(edibleStatusAlias);
                }
            }

            if (recognitionStatusAlias != null)
            {
                RecognitionStatus recognitionStatus = await _context.RecognitionStatuses.Where((status) => status.StatusAlias == recognitionStatusAlias).FirstOrDefaultAsync();

                if (recognitionStatus != null)
                {
                    requests = requests.Where((request) => request.StatusId == recognitionStatus.StatusId);
                }
                else
                {
                    return NotFound(recognitionStatusAlias);
                }
            }

            return Ok((await requests.ToArrayAsync())
                .Select((request) => ResponsedRequest(request)));
        }

        [HttpGet]
        public async Task<IActionResult> GetRecognitionRequest(uint id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var recognitionRequest = await _context.RecognitionRequests.FindAsync(id);

            if (recognitionRequest == null)
            {
                return NotFound();
            }

            return Ok(ResponsedRequest(recognitionRequest));
        }

        [HttpPost]
        public async Task<IActionResult> PostRecognitionRequest([FromBody] RecognitionRequest recognitionRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.RecognitionRequests.Add(recognitionRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRecognitionRequest", new { id = recognitionRequest.RequestId }, recognitionRequest);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRecognitionRequest(uint id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var recognitionRequest = await _context.RecognitionRequests.FindAsync(id);
            if (recognitionRequest == null)
            {
                return NotFound();
            }

            _context.RecognitionRequests.Remove(recognitionRequest);
            await _context.SaveChangesAsync();

            return Ok(ResponsedRequest(recognitionRequest));
        }
    }
}
