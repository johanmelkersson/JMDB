using JMDB.Services;
using Microsoft.AspNetCore.SignalR;

namespace JMDB.Hubs
{
    public class ReviewHub : Hub
    {
        private readonly UserTracker _tracker;

        public ReviewHub(UserTracker tracker)
        {
            _tracker = tracker;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (userId != null)
            {
                var count = _tracker.Add(userId);
                await Clients.All.SendAsync("ActiveUsersUpdated", count);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            if (userId != null)
            {
                var count = _tracker.Remove(userId);
                await Clients.All.SendAsync("ActiveUsersUpdated", count);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinMovieGroup(string movieId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"movie-{movieId}");
        }

        public async Task LeaveMovieGroup(string movieId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"movie-{movieId}");
        }
    }
}
