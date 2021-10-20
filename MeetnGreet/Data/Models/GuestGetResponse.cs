using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetnGreet.Data.Models
{
    public class GuestGetResponse
    {
        public int GuestId { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
        public DateTime Created { get; set; }
    }
}
