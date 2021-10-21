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
        public IEnumerable<MeetingGetManyResponse> GetMeetings(string search, bool includeGuests, int page = 1, int pageSize = 20)
        {
            if (string.IsNullOrEmpty(search))
            {
                if (includeGuests)
                {
                    return _dataRepository.GetMeetingsWithGuests();
                } else
                {
                    return _dataRepository.GetMeetings();
                }
            }
            else
            {
                return _dataRepository.GetMeetingsBySearchWithPaging(
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
        public ActionResult<MeetingGetSingleResponse> GetMeeting(int meetingId)
        {
            var meeting = _cache.Get(meetingId);
            if (meeting == null)
            {
                meeting = _dataRepository.GetMeeting(meetingId);
                if (meeting == null)
                {
                    return NotFound();
                }
                _cache.Set(meeting);
            }
            return meeting;
        }

        [HttpPost]
        public ActionResult<MeetingGetSingleResponse> PostMeeting(MeetingPostRequest meetingPostRequest)
        {
            var savedMeeting = _dataRepository.PostMeeting(new MeetingPostFullRequest
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

        [HttpPut("{meetingId}")]
        public ActionResult<MeetingGetSingleResponse> PutMeeting(int meetingId, MeetingPutRequest meetingPutRequest)
        {
            var meeting = _dataRepository.GetMeeting(meetingId);
            if (meeting == null)
            {
                return NotFound();
            }
            meetingPutRequest.Title = string.IsNullOrEmpty(meetingPutRequest.Title) ? meeting.Title : meetingPutRequest.Title;
            meetingPutRequest.Content = string.IsNullOrEmpty(meetingPutRequest.Content) ? meeting.Content : meetingPutRequest.Content;
            var savedMeeting = _dataRepository.PutMeeting(meetingId, meetingPutRequest);
            _cache.Remove(savedMeeting.MeetingId);
            
            return savedMeeting;
        }

        [HttpDelete("{meetingId}")]
        public ActionResult DeleteMeeting(int meetingId)
        {
            var meeting = _dataRepository.GetMeeting(meetingId);
            if (meeting == null)
            {
                return NotFound();
            }
            _dataRepository.DeleteMeeting(meetingId);
            _cache.Remove(meetingId);

            return NoContent();
        }

        [HttpPost("guest")]
        public ActionResult<GuestGetResponse> PostGuest(GuestPostRequest guestPostRequest)
        {
            var meetingExists = _dataRepository.MeetingExists(guestPostRequest.MeetingId.Value);
            if (!meetingExists)
            {
                return NotFound();
            }
            var savedGuest = _dataRepository.PostGuest(new GuestPostFullRequest
            {
                MeetingId = guestPostRequest.MeetingId.Value,
                Content = guestPostRequest.Content,
                UserId = "1",
                UserName = "bob.test@test.com",
                Created = DateTime.UtcNow
            }
            );
            _cache.Remove(guestPostRequest.MeetingId.Value);

            _meetingHubContext.Clients.Group(
                $"Meeting-{guestPostRequest.MeetingId.Value}")
                .SendAsync(
                    "ReceiveMeeting",
                    _dataRepository.GetMeeting(
                        guestPostRequest.MeetingId.Value));

            return savedGuest;
        }
    }
}
