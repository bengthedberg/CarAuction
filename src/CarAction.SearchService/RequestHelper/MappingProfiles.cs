using AutoMapper;

using CarAction.Contracts.Actions;
using CarAction.SearchService.Models;

namespace CarAction.SearchService;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<AuctionCreated, Item>();
    }
}
