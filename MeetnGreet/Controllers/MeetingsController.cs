using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeetnGreet.Data;
using MeetnGreet.Data.Models;

namespace MeetnGreet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingsController : ControllerBase
    {
        private readonly IDataRepository _dataRepository;

        public MeetingsController(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        [HttpGet]
        public IEnumerable<MeetingGetManyResponse> GetMeetings(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return _dataRepository.GetMeetings();
            }
            else
            {
                return _dataRepository.GetMeetingsBySearch(search);
            }
        }

        [HttpGet("unanswered")]
        public IEnumerable<MeetingGetManyResponse> GetUnansweredMeetings()
        {
            return _dataRepository.GetUnansweredMeetings();
        }

        [HttpGet("{meetingId}")]
        public ActionResult<MeetingGetSingleResponse> GetMeeting(int meetingId)
        {
            var meeting = _dataRepository.GetMeeting(meetingId);
            if (meeting == null)
            {
                return NotFound();
            }
            return meeting;
        }

        [HttpPost]
        public ActionResult<MeetingGetSingleResponse> PostMeeting(MeetingPostRequest meetingPostRequest)
        {
            var savedMeeting = _dataRepository.PostMeeting(meetingPostRequest);
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
            return NoContent();
        }

        [HttpPost("guest")]
        public ActionResult<GuestGetResponse> PostGuest(GuestPostRequest guestPostRequest)
        {
            var meetingExists = _dataRepository.MeetingExists(guestPostRequest.MeetingId);
            if (!meetingExists)
            {
                return NotFound();
            }
            var savedGuest = _dataRepository.PostGuest(guestPostRequest);
            return savedGuest;
        }
    }
}
