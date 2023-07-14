using signalr.Models;

namespace signalr.Repositories
{
    public interface IAuctionRepo
    {
        IEnumerable<Auction> GetAll();
        void NewBid(int auctionId, int newBid);
        void AddAuction(Auction auction);
    }
}