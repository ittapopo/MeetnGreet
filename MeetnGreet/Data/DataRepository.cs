using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;
using MeetnGreet.Data.Models;

namespace MeetnGreet.Data
{
    public class DataRepository : IDataRepository
    {
        private readonly string _connectionString;

        public DataRepository(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionString:DefaultConnection"];
        }
        public GuestGotResponse GetGuest(int answerId)
        {
            throw new NotImplementedException();
        }

        public MeetingGetSingleResponse GetMeeting(int meetingId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MeetingGetManyResponse> GetMeetings()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Query<MeetingGetManyResponse>(
                    @"EXEC dbo.Meeting_GetMany"
                );
            }
        }

        public IEnumerable<MeetingGetManyResponse> GetMeetingsBySearch(string search)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MeetingGetManyResponse> GetUnansweredMeetings()
        {
            throw new NotImplementedException();
        }

        public bool MeetingExists(int meetingId)
        {
            throw new NotImplementedException();
        }
    }
}
