using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MeetnGreet.Data.Models
{
    public class GuestPostRequest
    {
        [Required]
        public int? MeetingId { get; set; }
        [Required]
        public string Content { get; set; }
    }
}
