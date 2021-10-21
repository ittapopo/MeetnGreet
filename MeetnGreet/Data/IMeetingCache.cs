using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeetnGreet.Data.Models;

namespace MeetnGreet.Data
{
    public interface IMeetingCache
    {
        MeetingGetSingleResponse Get(int meetingId);
        void Remove(int meetingId);
        void Set(MeetingGetSingleResponse meeting);
    }
}
