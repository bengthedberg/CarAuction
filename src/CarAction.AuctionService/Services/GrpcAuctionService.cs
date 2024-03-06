using CarAction.AuctionService.Data;

using Grpc.Core;

namespace CarAction.AuctionService;
// GrpcAuction.GrpcAuctionBase is generated automatically from the proto file
public class GrpcAuctionService : GrpcAuction.GrpcAuctionBase
{
    private readonly AuctionDbContext _dbContext;

    public GrpcAuctionService(AuctionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // override the method we created in the proto file, GetAuctionRequest and GrpcAuctionResponse are defined there as
    // well as the method GetAuction
    public override async Task<GrpcAuctionResponse> GetAuction(GetAuctionRequest request,
        ServerCallContext context)
    {
        Console.WriteLine("==> Received Grpc request for auction");

        var auction = await _dbContext.Auctions.FindAsync(Guid.Parse(request.Id))
            ?? throw new RpcException(new Status(StatusCode.NotFound, "Not found"));

        var response = new GrpcAuctionResponse
        {
            Auction = new GrpcAuctionModel
            {
                AuctionEnd = auction.AuctionEnd.ToString(),
                Id = auction.Id.ToString(),
                ReservePrice = auction.ReservePrice,
                Seller = auction.Seller
            }
        };

        return response;
    }
}