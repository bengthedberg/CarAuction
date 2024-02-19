using CarAction.Contracts.Auctions;

using MassTransit;

namespace CarAction.AuctionService.Consumers;

public class AuctionCreatedErrorConsumer : IConsumer<Fault<AuctionCreated>>
{
    public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
    {
        Console.WriteLine($"----> Consuming the auction created error event {context.Message.Message.Id}");

        var exception = context.Message.Exceptions.First();
        switch (exception.ExceptionType)
        {
            // part of the contrived saab example
            case "System.ArgumentException":
                Console.WriteLine($"{exception.Message}");
                context.Message.Message.Make = "Volvo"; // change the make to 'Volvo
                await context.Publish(context.Message.Message); // republish the message
                break;
            default:
                Console.WriteLine($"----> Unhandled exception: {exception.Message}");
                break;
        }

        //TODO: Raise the event on a error dashboard
    }
}


