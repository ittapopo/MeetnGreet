using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetnGreet.Data.Models
{
    public class MeetingGetManyResponse
    {
        public int MeetingId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
        public DateTime Created { get; set; }
        public List<GuestGetResponse> Guests { get; set; }
    }
}
