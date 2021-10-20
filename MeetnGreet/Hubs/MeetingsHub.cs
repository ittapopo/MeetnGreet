using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace MeetnGreet.Hubs
{
    public class MeetingsHub: Hub
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Clients.Caller.SendAsync("Message",
                "Successfully connected");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.Caller.SendAsync("Message",
                "Successfully disconnected");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SubscribeMeeting(int meetingId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId,
                $"Meeting-{meetingId}");
            await Clients.Caller.SendAsync("Message",
                "Successfully subscribed");
        }

        public async Task UnsubscribeMeeting(int meetingId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId,
                $"Meeting-{meetingId}");
            await Clients.Caller.SendAsync("Message",
                "Successfully unsubscribed");
        }
    }
}
