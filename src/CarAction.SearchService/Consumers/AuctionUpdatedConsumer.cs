using AutoMapper;

using CarAction.Contracts.Auctions;
using CarAction.SearchService.Models;
using MongoDB.Entities;
using MassTransit;

namespace CarAction.SearchService;

public class AuctionUpdatedConsumer : IConsumer<AuctionUpdated>
{
    private readonly IMapper _mapper;

    public AuctionUpdatedConsumer(IMapper mapper)
    {
        _mapper = mapper;
    }
    public async Task Consume(ConsumeContext<AuctionUpdated> context)
    {
        Console.WriteLine($"----> Consuming the auction updated event {context.Message.Id}");

        var item = _mapper.Map<Item>(context.Message);

        var result = await DB.Update<Item>()
        .Match(a => a.ID == context.Message.Id)
        .ModifyOnly(b => new { b.Make, b.Model, b.Mileage, b.Color, b.Year }, item)
        .ExecuteAsync();

        if (!result.IsAcknowledged)
            throw new MessageException(typeof(AuctionUpdated), "Problem updating mongodb");
    }
}
