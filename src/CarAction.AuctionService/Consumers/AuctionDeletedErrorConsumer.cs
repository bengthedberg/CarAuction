using CarAction.Contracts.Auctions;

using MassTransit;

namespace CarAction.AuctionService.Consumers;

public class AuctionDeletedErrorConsumer : IConsumer<Fault<AuctionDeleted>>
{
    public async Task Consume(ConsumeContext<Fault<AuctionDeleted>> context)
    {
        Console.WriteLine($"----> Consuming the auction deleted error event {context.Message.Message.Id}");

        var exception = context.Message.Exceptions.First();
        switch (exception.ExceptionType)
        {
            default:
                Console.WriteLine($"----> Unhandled exception: {exception.Message}");
                break;
        }

        //TODO: Raise the event on a error dashboard
        await Task.Delay(0);
    }
}
