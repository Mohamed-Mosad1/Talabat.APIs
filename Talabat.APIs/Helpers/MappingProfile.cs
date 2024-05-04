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
                .ForMember(P => P.Brand, option => option.MapFrom(S => S.Brand.Name))
                .ForMember(P => P.Category, option => option.MapFrom(S => S.Category.Name))
                .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom<ProductPictureUrlResolver>());

            CreateMap<CustomerBasketDto, CustomerBasket>();
            CreateMap<BasketItemDto, BasketItem>();

            CreateMap<UserAddress, UserAddressDto>().ReverseMap();

            CreateMap<OrderAddressDto, OrderAddress>();

            CreateMap<Order, OrderToReturnDto>()
                .ForMember(dest => dest.DeliveryMethod, opt => opt.MapFrom(src => src.DeliveryMethod != null ? src.DeliveryMethod.ShortName : string.Empty))
                .ForMember(dest => dest.DeliveryMethodCoast, opt => opt.MapFrom(src => src.DeliveryMethod != null ? src.DeliveryMethod.Cost : 0));


            CreateMap<OrderItems, OrderItemsDto>()
                .ForMember(D => D.ProductId, option => option.MapFrom(S => S.Product.ProductId))
                .ForMember(D => D.ProductName, option => option.MapFrom(S => S.Product.ProductName))
                .ForMember(D => D.PictureUrl, option => option.MapFrom(S => S.Product.PictureUrl))
                .ForMember(D => D.PictureUrl, option => option.MapFrom<OrderItemPictureUrlResolver>());
        }
    }
}
