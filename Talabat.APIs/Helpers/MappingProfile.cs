using AutoMapper;
using Talabat.APIs.Dtos;
using Talabat.Core.Entities.Basket;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Entities.Orders_Aggregate;
using Talabat.Core.Entities.Product;

namespace Talabat.APIs.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(dest => dest.Brand, option => option.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.Category, option => option.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.PictureUrl, option => option.MapFrom<ProductPictureUrlResolver>());

            CreateMap<CustomerBasketDto, CustomerBasket>().ReverseMap();
            CreateMap<BasketItemDto, BasketItem>().ReverseMap();

            CreateMap<UserAddress, UserAddressDto>().ReverseMap();

            CreateMap<OrderAddressDto, OrderAddress>();

            CreateMap<Order, OrderToReturnDto>()
                .ForMember(dest => dest.DeliveryMethod, option => option.MapFrom(src => src.DeliveryMethod != null ? src.DeliveryMethod.ShortName : string.Empty))
                .ForMember(dest => dest.DeliveryMethodCoast, option => option.MapFrom(src => src.DeliveryMethod != null ? src.DeliveryMethod.Cost : 0));


            CreateMap<OrderItems, OrderItemsDto>()
                .ForMember(dest => dest.ProductId, option => option.MapFrom(src => src.Product.ProductId))
                .ForMember(dest => dest.ProductName, option => option.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.PictureUrl, option => option.MapFrom(src => src.Product.PictureUrl))
                .ForMember(dest => dest.PictureUrl, option => option.MapFrom<OrderItemPictureUrlResolver>());
        }
    }
}
