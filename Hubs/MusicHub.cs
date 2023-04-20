using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace HeyListen.Hubs
{
    public class MusicHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Console.WriteLine("A Client Connected: " + Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine("A client disconnected: " + Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task PlaySong(string song)
        {
            Console.WriteLine("Song Received: " + song);
        }

        public async Task AddToGroup(int groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());
        }

        public async Task RemoveFromGroup(int groupId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId.ToString());
        }
    }
}
