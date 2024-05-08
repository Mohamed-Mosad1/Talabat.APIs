using AdminDashboard.ViewModels;
using AutoMapper;
using Talabat.Core.Entities.Product;

namespace AdminDashboard.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductViewModel>()
    .ForMember(dest => dest.BrandId, opt => opt.MapFrom(src => src.BrandId))
    .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand.Name))
    .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
    .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name))
    .ReverseMap();


        }
    }
}
