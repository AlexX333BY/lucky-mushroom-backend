using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuckyMushroom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using LuckyMushroom.DataTransferObjects;
using LuckyMushroom.Helpers;

namespace LuckyMushroom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class RecognitionRequestsController : ControllerBase
    {
        private readonly LuckyMushroomContext _context;

        public RecognitionRequestsController(LuckyMushroomContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetRecognitionRequests(string edibleStatusAlias, string recognitionStatusAlias)
        {
            IQueryable<RecognitionRequest> requests = _context.RecognitionRequests.Where((request) => request.Requester.UserCredentials.UserMail == User.Identity.Name);

            if (edibleStatusAlias != null)
            {
                requests = requests.Where((request) => request.EdibleStatus.EdibleStatusAlias == edibleStatusAlias);
            }

            if (recognitionStatusAlias != null)
            {
                requests = requests.Where((request) => request.Status.StatusAlias == recognitionStatusAlias);
            }

            RecognitionRequest[] resultRequests = await requests.ToArrayAsync();
            EdibleStatus[] edibleStatuses = await _context.EdibleStatuses.ToArrayAsync();
            RecognitionStatus[] recognitionStatuses = await _context.RecognitionStatuses.ToArrayAsync();

            foreach (RecognitionRequest request in resultRequests)
            {
                request.RequestPhotos = await _context.RequestPhotos.Where((photo) => photo.RequestId == request.RequestId).ToArrayAsync();
                request.Status = recognitionStatuses.Single((status) => status.StatusId == request.StatusId);
                request.EdibleStatus = edibleStatuses.Single((status) => status.EdibleStatusId == request.EdibleStatusId);
            }

            return Ok(resultRequests.Select((request) => new RecognitionRequestDto(request)).ToArray());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRecognitionRequest(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            RecognitionRequest recognitionRequest = await _context.RecognitionRequests
                .Where((request) => (request.RequestId == id) && (request.Requester.UserCredentials.UserMail == User.Identity.Name))
                .SingleOrDefaultAsync();

            if (recognitionRequest == null)
            {
                return NotFound();
            }

            recognitionRequest.RequestPhotos = await _context.RequestPhotos.Where((photo) => photo.RequestId == recognitionRequest.RequestId).ToArrayAsync();
            recognitionRequest.Status = await _context.RecognitionStatuses.FindAsync(recognitionRequest.StatusId);
            recognitionRequest.EdibleStatus = await _context.EdibleStatuses.FindAsync(recognitionRequest.EdibleStatusId);

            return Ok(new RecognitionRequestDto(recognitionRequest));
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddRecognitionRequest([FromBody] RecognitionRequestDto recognitionRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                if (!IsRequestCorrect(recognitionRequest))
                {
                    return BadRequest("Request in incorrect");
                }

                RecognitionRequest request = (await _context.RecognitionRequests.AddAsync(new RecognitionRequest() {
                    StatusId = (await _context.RecognitionStatuses.SingleAsync((status) => status.StatusAlias == recognitionRequest.RecognitionStatus.RecognitionStatusAlias)).StatusId,
                    EdibleStatusId = (await _context.EdibleStatuses.SingleAsync((status) => status.EdibleStatusAlias == recognitionRequest.EdibleStatus.EdibleStatusAlias)).EdibleStatusId,
                    RequestDatetime = recognitionRequest.RequestDatetime.Value,
                    RequesterId = (await _context.UserCredentials.SingleAsync((creds) => creds.UserMail == User.Identity.Name)).UserId
                })).Entity;

                List<Task> photoAdditionTasks = new List<Task>(recognitionRequest.RequestPhotos.Length); 
                foreach (RequestPhotoDto photo in recognitionRequest.RequestPhotos)
                {
                    photoAdditionTasks.Add(_context.AddAsync(new RequestPhoto { RequestId = request.RequestId, PhotoFilename = new RequestPhotoSaver().SavePhoto(photo.PhotoData, photo.PhotoExtension) }));
                }
                await Task.WhenAll(photoAdditionTasks);

                await _context.SaveChangesAsync();

                transaction.Commit();
                return CreatedAtAction(nameof(AddRecognitionRequest), new RecognitionRequestDto(request));
            }
        }

        [HttpDelete("{id}/delete")]
        public async Task<IActionResult> DeleteRecognitionRequest(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            RecognitionRequest recognitionRequest = await _context.RecognitionRequests
                .Where((request) => (request.RequestId == id) && (request.Requester.UserCredentials.UserMail == User.Identity.Name))
                .SingleOrDefaultAsync();
            if (recognitionRequest == null)
            {
                return NotFound();
            }

            _context.RecognitionRequests.Remove(recognitionRequest);
            await _context.SaveChangesAsync();

            return Ok(new RecognitionRequestDto(recognitionRequest));
        }

        protected bool IsRequestCorrect(RecognitionRequestDto request)
        {
            if (request == null)
            {
                return false;
            }

            if ((request.EdibleStatus == null) || (request.RecognitionStatus == null) || (request.RequestDatetime == null) || (request.RequestPhotos == null)
                || (request.EdibleStatus.EdibleStatusAlias == null) || (request.RecognitionStatus.RecognitionStatusAlias == null) || (request.RequestPhotos.Length == 0))
            {
                return false;
            }

            if (_context.RecognitionStatuses.Where((status) => status.StatusAlias == request.RecognitionStatus.RecognitionStatusAlias).Count() == 0)
            {
                return false;
            }

            if (_context.EdibleStatuses.Where((status) => status.EdibleStatusAlias == request.EdibleStatus.EdibleStatusAlias).Count() == 0)
            {
                return false;
            }

            if (request.RequestPhotos.Any((photo) => photo.PhotoData == null || photo.PhotoData.Length == 0 || photo.PhotoExtension == null || photo.PhotoExtension.Length == 0))
            {
                return false;
            }

            return true;
        }
    }
}
