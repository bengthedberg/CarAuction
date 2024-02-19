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

        // Contrived example to throw exception if the item is a Saab
        if (item.Make == "Saab")
        {
            throw new ArgumentException("Saab is not allowed");
        }

        await item.SaveAsync();
    }
}
