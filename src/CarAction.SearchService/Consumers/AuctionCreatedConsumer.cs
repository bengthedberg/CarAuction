using AutoMapper;

using CarAction.Contracts.Actions;
using CarAction.SearchService.Models;

using MassTransit;

using MongoDB.Entities;

namespace CarAction.SearchService;

public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
{
    private readonly IMapper _mapper;

    public AuctionCreatedConsumer(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<AuctionCreated> context)
    {
        Console.WriteLine($"----> Consuming the auction created event {context.Message.Id}");

        var item = _mapper.Map<Item>(context.Message);

        await item.SaveAsync();
    }
}
