using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http.Connections;

var serverUrl = args.Length > 0 ? args[0] : "http://localhost";

using var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri($"{serverUrl}");
var response = await httpClient.GetAsync("/auctions");
var auctions = await response.Content.ReadFromJsonAsync<Auction[]>();

if (auctions == null)
    return;

foreach (var auction in auctions)
{
    Console.WriteLine($"{auction.Id,-3} {auction.ItemName,-20} {auction.CurrentBid,10}");
}

var connection = new HubConnectionBuilder()
    .WithUrl($"{serverUrl}/auctionhub", o =>
        {
            o.Transports = HttpTransportType.WebSockets;
            o.SkipNegotiation = true;
        }).Build();

connection.On("ReceiveNewBid", (AuctionNotify auctionNotify) => {
    var auction = auctions.Single(a => a.Id == auctionNotify.AuctionId);
    auction.CurrentBid = auctionNotify.NewBid;
    Console.WriteLine("New bid:");
    Console.WriteLine($"{auction.Id,-3} {auction.ItemName,-20} {auction.CurrentBid,10}");
});

connection.On("ReceiveNewAuction", (Auction auction) => {
    Console.WriteLine("New auction:");
    Console.WriteLine($"{auction.Id,-3} {auction.ItemName,-20} {auction.CurrentBid,10}");
});

try
{
    await connection.StartAsync();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

try
{
    while (true)
    {
        Console.WriteLine("Auction id?");
        var id = Console.ReadLine();
        Console.WriteLine($"New bid for auction {id}?");
        var bid = Console.ReadLine();
        await connection.InvokeAsync("NotifyNewBid",
            new { AuctionId = int.Parse(id!), NewBid = int.Parse(bid!) });
        Console.WriteLine("Bid placed");
    }
}
finally
{
    await connection.StopAsync();
}