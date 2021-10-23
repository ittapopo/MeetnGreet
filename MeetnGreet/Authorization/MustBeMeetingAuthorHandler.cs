using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using MeetnGreet.Data;

namespace MeetnGreet.Authorization
{
    public class MustBeMeetingAuthorHandler : AuthorizationHandler<MustBeMeetingAuthorRequirement>
    {
        private readonly IDataRepository _dataRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MustBeMeetingAuthorHandler(IDataRepository dataRepository, IHttpContextAccessor httpContextAccessor)
        {
            _dataRepository = dataRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, MustBeMeetingAuthorRequirement requirement)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return;
            }

            var meetingId = _httpContextAccessor.HttpContext.Request.RouteValues["meetingId"];
            int meetingIdAsInt = Convert.ToInt32(meetingId);

            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var meeting =  await _dataRepository.GetMeeting(meetingIdAsInt);
            if (meeting == null)
            {
                //let it through so the controller can return a 404
                context.Succeed(requirement);
                return;
            }

            if (meeting.UserId != userId)
            {
                context.Fail();
                return;
            }

            context.Succeed(requirement);
        }
    }
}
