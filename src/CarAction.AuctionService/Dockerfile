FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /app
EXPOSE 80

# copy all .csproj files and restore as distinct layers.   Use of the same COPY command
# for every dockerfile in the project to take advantage of docker caching
COPY CarAuction.sln CarAuction.sln
COPY src/CarAction.AuctionService/CarAction.AuctionService.csproj src/CarAction.AuctionService/CarAction.AuctionService.csproj
COPY src/CarAction.SearchService/CarAction.SearchService.csproj src/CarAction.SearchService/CarAction.SearchService.csproj
COPY src/GatewayService/GatewayService.csproj src/GatewayService/GatewayService.csproj
#COPY src/CarAction.BiddingService/CarAction.BiddingService.csproj src/CarAction.BiddingService/CarAction.BiddingService.csproj
#COPY src/CarAction.NotificationService/CarAction.NotificationService.csproj src/CarAction.NotificationService/CarAction.NotificationService.csproj
COPY src/CarAction.Contracts/CarAction.Contracts.csproj src/CarAction.Contracts/CarAction.Contracts.csproj
COPY src/IdentityService/IdentityService.csproj src/IdentityService/IdentityService.csproj
#COPY tests/AuctionService.UnitTests/AuctionService.UnitTests.csproj tests/AuctionService.UnitTests/AuctionService.UnitTests.csproj
#COPY tests/AuctionService.IntegrationTests/AuctionService.IntegrationTests.csproj tests/AuctionService.IntegrationTests/AuctionService.IntegrationTests.csproj

# Restore package dependecies
RUN dotnet restore CarAuction.sln

# Copy the micro service folders over
COPY src/CarAction.AuctionService src/CarAction.AuctionService
COPY src/CarAction.Contracts src/CarAction.Contracts
WORKDIR /app/src/CarAction.AuctionService
RUN dotnet publish -c Release -o /app/src/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/src/out .
ENTRYPOINT [ "dotnet", "CarAction.AuctionService.dll" ]