using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeetnGreet.Data.Models;

namespace MeetnGreet.Data
{
    public interface IDataRepository
    {
        IEnumerable<MeetingGetManyResponse> GetMeetings();

        IEnumerable<MeetingGetManyResponse> GetMeetingsWithGuests();

        IEnumerable<MeetingGetManyResponse> GetMeetingsBySearch(string search);
        IEnumerable<MeetingGetManyResponse> GetMeetingsBySearchWithPaging(string search, int pageNumber, int pageSize);

        IEnumerable<MeetingGetManyResponse> GetUnansweredMeetings();
        Task<IEnumerable<MeetingGetManyResponse>> GetUnansweredMeetingsAsync();

        MeetingGetSingleResponse GetMeeting(int meetingId);

        bool MeetingExists(int meetingId);

        GuestGetResponse GetGuest(int answerId);

        MeetingGetSingleResponse PostMeeting(MeetingPostFullRequest meeting);

        MeetingGetSingleResponse PutMeeting(int meetingId, MeetingPutRequest meeting);

        void DeleteMeeting(int meetingId);

        GuestGetResponse PostGuest(GuestPostFullRequest guest);
    }
}
