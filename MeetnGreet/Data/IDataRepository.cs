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

        IEnumerable<MeetingGetManyResponse> GetMeetingsBySearch(string search);

        IEnumerable<MeetingGetManyResponse> GetUnansweredMeetings();

        MeetingGetSingleResponse GetMeeting(int meetingId);

        bool MeetingExists(int meetingId);

        GuestGotResponse GetGuest(int answerId);
    }
}
