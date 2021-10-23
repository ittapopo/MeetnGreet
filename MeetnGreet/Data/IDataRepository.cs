using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeetnGreet.Data.Models;

namespace MeetnGreet.Data
{
    public interface IDataRepository
    {
        Task<IEnumerable<MeetingGetManyResponse>> GetMeetings();

        Task<IEnumerable<MeetingGetManyResponse>> GetMeetingsWithGuests();

        Task<IEnumerable<MeetingGetManyResponse>> GetMeetingsBySearch(string search);
        Task<IEnumerable<MeetingGetManyResponse>> GetMeetingsBySearchWithPaging(string search, int pageNumber, int pageSize);

        Task<IEnumerable<MeetingGetManyResponse>> GetUnansweredMeetings();
        Task<IEnumerable<MeetingGetManyResponse>> GetUnansweredMeetingsAsync();

        Task<MeetingGetSingleResponse> GetMeeting(int meetingId);

        Task<bool> MeetingExists(int meetingId);

        Task<GuestGetResponse> GetGuest(int answerId);

        Task<MeetingGetSingleResponse> PostMeeting(MeetingPostFullRequest meeting);

        Task<MeetingGetSingleResponse> PutMeeting(int meetingId, MeetingPutRequest meeting);

        Task DeleteMeeting(int meetingId);

        Task<GuestGetResponse> PostGuest(GuestPostFullRequest guest);
    }
}
