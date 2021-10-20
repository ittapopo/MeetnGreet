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
    }
}
