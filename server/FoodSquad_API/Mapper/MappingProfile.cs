using AutoMapper;
using FoodSquad_API.Models.DTO;
using FoodSquad_API.Models.Entity;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Order and OrderDTO
        CreateMap<Order, OrderDTO>()
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.MenuItemQuantities, opt => opt.MapFrom(src =>
                src.MenuItemsWithQuantity.ToDictionary(m => m.MenuItem.Id, m => m.Quantity)))
            .ReverseMap();
    }
}
