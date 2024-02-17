# Auction Service


## Database Migrations

### Prerequisites

You will need the dotnet-ef tool

```
dotnet tool list -g
Package Id                           Version       Commands
------------------------------------------------------------------------------
dotnet-ef                            8.0.2         dotnet-ef
```

To install it:
`dotnet tool install dotnet-ef -g`

To upgrade it:
`dotnet tool update dotnet-ef -g`


## Migrate

> Ensure you are in the CarAuction.AuctionService folder.

Run the following command to create a entioty framework migration:
`dotnet ef migrations add "InitialCreate" -o Data/Migrations`

## Apply Migration

> Ensure you have a PostGres database and the connection string is correct.

`dotent ef database update`

## Drop database

`dotnet ef database drop`
