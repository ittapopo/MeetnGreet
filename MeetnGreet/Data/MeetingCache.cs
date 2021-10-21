using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using MeetnGreet.Data.Models;

namespace MeetnGreet.Data
{
    public class MeetingCache: IMeetingCache
    {
        private MemoryCache _cache { get; set; }
        public MeetingCache()
        {
            _cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 100
            });
        }

        public string GetCacheKey(int meetingId) => $"Meeting-{meetingId}";

        public MeetingGetSingleResponse Get(int meetingId)
        {
            MeetingGetSingleResponse meeting;
            _cache.TryGetValue(GetCacheKey(meetingId), out meeting);
            return meeting;
        }

        public void Set(MeetingGetSingleResponse meeting)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1);
            _cache.Set(GetCacheKey(meeting.MeetingId), meeting, cacheEntryOptions);
        }

        public void Remove(int meetingId)
        {
            _cache.Remove(GetCacheKey(meetingId));
        }
    }
}
