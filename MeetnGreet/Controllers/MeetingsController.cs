using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeetnGreet.Data;
using MeetnGreet.Data.Models;
using Microsoft.AspNetCore.SignalR;
using MeetnGreet.Hubs;
using Microsoft.AspNetCore.Authorization;

namespace MeetnGreet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingsController : ControllerBase
    {
        private readonly IDataRepository _dataRepository;
        private readonly IHubContext<MeetingsHub> _meetingHubContext;
        private readonly IMeetingCache _cache;

        public MeetingsController(IDataRepository dataRepository, IHubContext<MeetingsHub> meetingHubContext, IMeetingCache meetingCache)
        {
            _dataRepository = dataRepository;
            _meetingHubContext = meetingHubContext;
            _cache = meetingCache;
        }

        [HttpGet]
        public async Task<IEnumerable<MeetingGetManyResponse>> GetMeetings(string search, bool includeGuests, int page = 1, int pageSize = 20)
        {
            if (string.IsNullOrEmpty(search))
            {
                if (includeGuests)
                {
                    return await _dataRepository.GetMeetingsWithGuests();
                } else
                {
                    return await _dataRepository.GetMeetings();
                }
            }
            else
            {
                return await _dataRepository.GetMeetingsBySearchWithPaging(
                    search,
                    page,
                    pageSize
                    );
            }
        }

        [HttpGet("unanswered")]
        public async Task<IEnumerable<MeetingGetManyResponse>> GetUnansweredMeetings()
        {
            return await _dataRepository.GetUnansweredMeetingsAsync();
        }

        [HttpGet("{meetingId}")]
        public async Task<ActionResult<MeetingGetSingleResponse>> GetMeeting(int meetingId)
        {
            var meeting = _cache.Get(meetingId);
            if (meeting == null)
            {
                meeting = await _dataRepository.GetMeeting(meetingId);
                if (meeting == null)
                {
                    return NotFound();
                }
                _cache.Set(meeting);
            }
            return meeting;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<MeetingGetSingleResponse>> PostMeeting(MeetingPostRequest meetingPostRequest)
        {
            var savedMeeting = await _dataRepository.PostMeeting(new MeetingPostFullRequest
            {
                Title = meetingPostRequest.Title,
                Content = meetingPostRequest.Content,
                UserId = "1",
                UserName = "bob.test@test.com",
                Created = DateTime.UtcNow
            });
            return CreatedAtAction(nameof(GetMeeting),
                new { meetingId = savedMeeting.MeetingId },
                savedMeeting);
        }

        [Authorize(Policy ="MustBeMeetingAuthor")]
        [HttpPut("{meetingId}")]
        public async Task<ActionResult<MeetingGetSingleResponse>> PutMeeting(int meetingId, MeetingPutRequest meetingPutRequest)
        {
            var meeting = await _dataRepository.GetMeeting(meetingId);
            if (meeting == null)
            {
                return NotFound();
            }
            meetingPutRequest.Title = string.IsNullOrEmpty(meetingPutRequest.Title) ? meeting.Title : meetingPutRequest.Title;
            meetingPutRequest.Content = string.IsNullOrEmpty(meetingPutRequest.Content) ? meeting.Content : meetingPutRequest.Content;
            var savedMeeting = await _dataRepository.PutMeeting(meetingId, meetingPutRequest);
            _cache.Remove(savedMeeting.MeetingId);
            
            return savedMeeting;
        }

        [Authorize(Policy = "MustBeMeetingAuthor")]
        [HttpDelete("{meetingId}")]
        public async Task<ActionResult> DeleteMeeting(int meetingId)
        {
            var meeting = await _dataRepository.GetMeeting(meetingId);
            if (meeting == null)
            {
                return NotFound();
            }
            await _dataRepository.DeleteMeeting(meetingId);
            _cache.Remove(meetingId);

            return NoContent();
        }

        [Authorize]
        [HttpPost("guest")]
        public async Task<ActionResult<GuestGetResponse>> PostGuest(GuestPostRequest guestPostRequest)
        {
            var meetingExists = await _dataRepository.MeetingExists(guestPostRequest.MeetingId.Value);
            if (!meetingExists)
            {
                return NotFound();
            }
            var savedGuest = await _dataRepository.PostGuest(new GuestPostFullRequest
            {
                MeetingId = guestPostRequest.MeetingId.Value,
                Content = guestPostRequest.Content,
                UserId = "1",
                UserName = "bob.test@test.com",
                Created = DateTime.UtcNow
            }
            );
            _cache.Remove(guestPostRequest.MeetingId.Value);

            await _meetingHubContext.Clients.Group(
                $"Meeting-{guestPostRequest.MeetingId.Value}")
                .SendAsync(
                    "ReceiveMeeting",
                    _dataRepository.GetMeeting(
                        guestPostRequest.MeetingId.Value));

            return savedGuest;
        }
    }
}
