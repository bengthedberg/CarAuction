﻿using CarAction.AuctionService.Data;
using CarAction.AuctionService.Entities;
using CarAction.Contracts.Auctions;

using MassTransit;

namespace CarAction.AuctionService.Consumers;

public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
{
    private readonly AuctionDbContext _dbContext;

    public AuctionFinishedConsumer(AuctionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<AuctionFinished> context)
    {
        Console.WriteLine("----> Consuming auction finished");

        var auction = await _dbContext.Auctions.FindAsync(Guid.Parse(context.Message.AuctionId));

        if (context.Message.ItemSold)
        {
            auction.Winner = context.Message.Winner;
            auction.SoldAmount = context.Message.Amount;
        }

        auction.Status = auction.SoldAmount > auction.ReservePrice
            ? Status.Finished : Status.ReserveNotMet;

        await _dbContext.SaveChangesAsync();
    }
}
