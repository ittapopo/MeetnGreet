using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;
using MeetnGreet.Data.Models;
using static Dapper.SqlMapper;

namespace MeetnGreet.Data
{
    public class DataRepository : IDataRepository
    {
        private readonly string _connectionString;

        public DataRepository(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionStrings:DefaultConnection"];
        }
        public GuestGetResponse GetGuest(int guestId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.QueryFirstOrDefault<GuestGetResponse>(
                    @"EXEC dbo.Guest_Get_ByGuestId @GuestId = @GuestId",
                    new { GuestId = guestId }
                    );
            }
        }

        public MeetingGetSingleResponse GetMeeting(int meetingId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (GridReader results =
                    connection.QueryMultiple(
                        @"EXEC dbo.Meeting_GetSingle
                            @MeetingId = @MeetingId;
                        EXEC dbo.Guest_Get_ByMeetingId
                            @MeetingId = @MeetingId",
                        new { MeetingId = meetingId }
                        )
                    )
                {
                    var meeting =
                        results.Read<MeetingGetSingleResponse>().FirstOrDefault();
                    if (meeting != null)
                    {
                        meeting.Guests = results.Read<GuestGetResponse>().ToList();
                    }
                    return meeting;
                }
            }
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

        public IEnumerable<MeetingGetManyResponse> GetMeetingsWithGuests()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var meetingDictionary =
                    new Dictionary<int, MeetingGetManyResponse>();
                return connection
                    .Query<
                        MeetingGetManyResponse,
                        GuestGetResponse,
                        MeetingGetManyResponse>(
                            "EXEC dbo.Meeting_GetMany_WithGuests",
                            map: (q, a) =>
                            {
                                MeetingGetManyResponse meeting;

                                if (!meetingDictionary.TryGetValue(q.MeetingId, out meeting))
                                {
                                    meeting = q;
                                    meeting.Guests = new List<GuestGetResponse>();
                                    meetingDictionary.Add(meeting.MeetingId, meeting);
                                }
                                meeting.Guests.Add(a);
                                return meeting;
                            },
                            splitOn: "MeetingId"
                            )
                        .Distinct()
                        .ToList();
            }
        }

        public IEnumerable<MeetingGetManyResponse> GetMeetingsBySearch(string search)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Query<MeetingGetManyResponse>(
                    @"Exec dbo.Meeting_GetMany_BySearch @Search = @Search",
                    new { Search = search }
                    );
            }
        }

        public IEnumerable<MeetingGetManyResponse> GetMeetingsBySearchWithPaging(string search, int pageNumber, int pageSize)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var parameters = new
                {
                    Search = search,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
                return connection.Query<MeetingGetManyResponse>(
                    @"EXEC dbo.Meeting_GetMany_BySearch_WithPaging
                        @Search = @Search,
                        @PageNumber = @PageNumber,
                        @PageSize = @PageSize", parameters
                    );
            }
        }

        public IEnumerable<MeetingGetManyResponse> GetUnansweredMeetings()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Query<MeetingGetManyResponse>(
                    "EXEC dbo.Meeting_GetUnanswered"
                    );
            }
        }

        public async Task<IEnumerable<MeetingGetManyResponse>> GetUnansweredMeetingsAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await
                    connection.QueryAsync<MeetingGetManyResponse>(
                        "EXEC dbo.Meeting_GetUnanswered");
            }
        }

        public bool MeetingExists(int meetingId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.QueryFirst<bool>(
                    @"EXEC dbo.Meeting_Exists @MeetingId = @MeetingId",
                    new { MeetingId = meetingId }
                    );
            }
        }

        public MeetingGetSingleResponse PostMeeting(MeetingPostFullRequest meeting)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var meetingId = connection.QueryFirst<int>(
                    @"EXEC dbo.Meeting_Post
                      @Title = @Title, @Content = @Content,
                      @UserId = @UserId, @UserName = @UserName,
                      @Created = @Created",
                    meeting
                    );

                return GetMeeting(meetingId);
            }
        }

        public MeetingGetSingleResponse PutMeeting(int meetingId, MeetingPutRequest meeting)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(
                    @"EXEC dbo.Meeting_Put
                      @MeetingId = @MeetingId, @Title = @Title, @Content = @Content",
                    new { MeetingId = meetingId, meeting.Title, meeting.Content }
                    );
                return GetMeeting(meetingId);
            }
        }

        public void DeleteMeeting(int meetingId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(
                    @"EXEC dbo.Meeting_Delete
                      @MeetingId = @MeetingId",
                    new { MeetingId = meetingId }
                    );
            }
        }

        public GuestGetResponse PostGuest(GuestPostFullRequest guest)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.QueryFirst<GuestGetResponse>(
                    @"EXEC dbo.Guest_Post
                      @MeetingId = @MeetingId, @Content = @Content,
                      @UserId = @UserId, @UserName = @UserName,
                      @Created = @Created",
                    guest
                    );
            }
        }
    }
}
