using Microsoft.AspNetCore.SignalR;
using ps_globomantics_signalr.Models;

namespace ps_globomantics_signalr.Hubs
{
    public class AuctionHub: Hub
    {
        public async Task NotifyNewBid(AuctionNotify auction)
        {
            var groupName = $"auction-{auction.AuctionId}";
           
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.OthersInGroup(groupName).SendAsync("NotifyOutbid", auction);
            
            await Clients.All.SendAsync("ReceiveNewBid", auction);
        }

        public override Task OnConnectedAsync()
        {
            var id = Context.ConnectionId;
            Console.WriteLine($"\n>>>>>>>>> connetionId = ${id} \n");
            return Task.CompletedTask;
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var id = Context.ConnectionId;
            Console.WriteLine($"\n<<<<<<<<< connetionId = ${id} \n");
            return base.OnDisconnectedAsync(exception);
        }
    }
}
