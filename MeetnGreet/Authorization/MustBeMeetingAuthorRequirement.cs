using Microsoft.AspNetCore.Authorization;

namespace MeetnGreet.Authorization
{
    public class MustBeMeetingAuthorRequirement : IAuthorizationRequirement
    {
        public MustBeMeetingAuthorRequirement()
        {
        }
    }
}
