using AutoMapper;

using CarAction.Contracts.Auctions;
using CarAction.SearchService.Models;
using MongoDB.Entities;
using MassTransit;

namespace CarAction.SearchService;

public class AuctionDeletedConsumer : IConsumer<AuctionDeleted>
{
    private readonly IMapper _mapper;

    public AuctionDeletedConsumer(IMapper mapper)
    {
        _mapper = mapper;
    }
    public async Task Consume(ConsumeContext<AuctionDeleted> context)
    {
        Console.WriteLine($"----> Consuming the auction deleted event {context.Message.Id}");

        var result = await DB.DeleteAsync<Item>(context.Message.Id);

        if (!result.IsAcknowledged)
            throw new MessageException(typeof(AuctionDeleted), "Problem deleting auction");
    }
}
