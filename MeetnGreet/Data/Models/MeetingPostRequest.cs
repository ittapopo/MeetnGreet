using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MeetnGreet.Data.Models
{
    public class MeetingPostRequest
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        [Required(ErrorMessage =
            "Please include some content for the meeting")]
        public string Content { get; set; }
    }
}
