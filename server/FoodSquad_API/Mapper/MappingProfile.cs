using AutoMapper;
using FoodSquad_API.Models.DTO.MenuItem;
using FoodSquad_API.Models.DTO.Order;
using FoodSquad_API.Models.DTO.Review;
using FoodSquad_API.Models.Entity;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Order, OrderDTO>()
         .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
         .ForMember(dest => dest.MenuItemQuantities, opt => opt.MapFrom(src =>
             src.MenuItemsWithQuantity.ToDictionary(
                 menuItem => menuItem.MenuItemId,
                 menuItem => menuItem.Quantity)))
         .ReverseMap()
         .ForPath(src => src.MenuItemsWithQuantity, opt => opt.MapFrom(dest =>
             dest.MenuItemQuantities.Select(kv => new OrderMenuItem
             {
                 MenuItemId = kv.Key,
                 Quantity = kv.Value
             }).ToList()));

        CreateMap<ReviewCreateDTO, Review>();
        CreateMap<ReviewUpdateDTO, Review>();
        CreateMap<Review, ReviewDTO>()
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.User.ImageUrl));

        CreateMap<MenuItemCreateDTO, MenuItem>();
        CreateMap<MenuItem, MenuItemDTO>();
        CreateMap<MenuItemDTO, MenuItem>().ReverseMap(); 
        CreateMap<MenuItemCreateDTO, MenuItem>();
        CreateMap<MenuItemUpdateDTO, MenuItem>();
        CreateMap<MenuItem, MenuItemUpdateDTO>();

        CreateMap<OrderCreateDTO, Order>();
        CreateMap<OrderUpdateDTO, Order>();

    }
}
