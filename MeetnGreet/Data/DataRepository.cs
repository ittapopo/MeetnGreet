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
        public async Task<GuestGetResponse> GetGuest(int guestId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryFirstOrDefaultAsync<GuestGetResponse>(
                    @"EXEC dbo.Guest_Get_ByGuestId @GuestId = @GuestId",
                    new { GuestId = guestId }
                    );
            }
        }

        public async Task<MeetingGetSingleResponse> GetMeeting(int meetingId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (GridReader results =
                    await connection.QueryMultipleAsync(
                        @"EXEC dbo.Meeting_GetSingle
                            @MeetingId = @MeetingId;
                        EXEC dbo.Guest_Get_ByMeetingId
                            @MeetingId = @MeetingId",
                        new { MeetingId = meetingId }
                        )
                    )
                {
                    var meeting =
                        (await results.ReadAsync<MeetingGetSingleResponse>()).FirstOrDefault();
                    if (meeting != null)
                    {
                        meeting.Guests = (await results.ReadAsync<GuestGetResponse>()).ToList();
                    }
                    return meeting;
                }
            }
        }

        public async Task<IEnumerable<MeetingGetManyResponse>> GetMeetings()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<MeetingGetManyResponse>(
                    @"EXEC dbo.Meeting_GetMany"
                );
            }
        }

        public async Task<IEnumerable<MeetingGetManyResponse>> GetMeetingsWithGuests()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var meetingDictionary =
                    new Dictionary<int, MeetingGetManyResponse>();
                return (await connection
                    .QueryAsync<
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
                            ))
                        .Distinct()
                        .ToList();
            }
        }

        public async Task<IEnumerable<MeetingGetManyResponse>> GetMeetingsBySearch(string search)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<MeetingGetManyResponse>(
                    @"Exec dbo.Meeting_GetMany_BySearch @Search = @Search",
                    new { Search = search }
                    );
            }
        }

        public async Task<IEnumerable<MeetingGetManyResponse>> GetMeetingsBySearchWithPaging(string search, int pageNumber, int pageSize)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var parameters = new
                {
                    Search = search,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
                return await connection.QueryAsync<MeetingGetManyResponse>(
                    @"EXEC dbo.Meeting_GetMany_BySearch_WithPaging
                        @Search = @Search,
                        @PageNumber = @PageNumber,
                        @PageSize = @PageSize", parameters
                    );
            }
        }

        public async Task<IEnumerable<MeetingGetManyResponse>> GetUnansweredMeetings()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<MeetingGetManyResponse>(
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

        public async Task<bool> MeetingExists(int meetingId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryFirstAsync<bool>(
                    @"EXEC dbo.Meeting_Exists @MeetingId = @MeetingId",
                    new { MeetingId = meetingId }
                    );
            }
        }

        public async Task<MeetingGetSingleResponse> PostMeeting(MeetingPostFullRequest meeting)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var meetingId = await connection.QueryFirstAsync<int>(
                    @"EXEC dbo.Meeting_Post
                      @Title = @Title, @Content = @Content,
                      @UserId = @UserId, @UserName = @UserName,
                      @Created = @Created",
                    meeting
                    );

                return await GetMeeting(meetingId);
            }
        }

        public async Task<MeetingGetSingleResponse> PutMeeting(int meetingId, MeetingPutRequest meeting)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                await connection.ExecuteAsync(
                    @"EXEC dbo.Meeting_Put
                      @MeetingId = @MeetingId, @Title = @Title, @Content = @Content",
                    new { MeetingId = meetingId, meeting.Title, meeting.Content }
                    );
                return await GetMeeting(meetingId);
            }
        }

        public async Task DeleteMeeting(int meetingId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                await connection.ExecuteAsync(
                    @"EXEC dbo.Meeting_Delete
                      @MeetingId = @MeetingId",
                    new { MeetingId = meetingId }
                    );
            }
        }

        public async Task<GuestGetResponse> PostGuest(GuestPostFullRequest guest)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryFirstAsync<GuestGetResponse>(
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
